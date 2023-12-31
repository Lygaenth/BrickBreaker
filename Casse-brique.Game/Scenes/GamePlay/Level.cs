using Cassebrique.Domain.API;
using Cassebrique.Domain.Bricks;
using Cassebrique.Factory;
using Cassebrique.Locators;
using Cassebrique.Scenes.UI;
using Godot;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

public partial class Level : Node2D
{
    #region SubNodes
    private MainUI _mainUI;
    private BarControl _barControl;
    private Marker2D _barStartMarker;
    private Marker2D _ballStartMarker;
    private Marker2D _trackerStartMarker;
    public Path2D BossPath { get; set; }
    public PathFollow2D BossPathFollow { get; set; }
    #endregion

    [Signal]
    public delegate void OnFinalScoreEventHandler(int score);

    [Export]
    public int Lives { get; set; }

    public int Score { get; set; }

    private int _numberOfBricks = 0;
    private int _numberOfBallsCreated = 0;
    private int _currentStage = 1;
    private Boss _boss;

    private readonly List<Ball> _balls;
    private readonly List<BonusTracker> _bonusTrackers;


    private IBrickFactory _brickFactory;
    private IBallFactory _ballFactory;
    private IProjectileFactory _projectileFactory;
    private ILevelService _levelService;

    /// <summary>
    /// Constructor
    /// </summary>
    public Level()
    {
        _balls = new List<Ball>();
        _bonusTrackers = new List<BonusTracker>();
    }

    /// <summary>
    /// Initializing values since it cannot be passed through constructor
    /// </summary>
    /// <param name="levelService"></param>
    /// <param name="brickFactory"></param>
    /// <param name="ballFactory"></param>
    /// <param name="projectileFactory"></param>
    public void Setup(ILevelService levelService, IBrickFactory brickFactory, IBallFactory ballFactory, IProjectileFactory projectileFactory)
    {
        _levelService = levelService;
        _brickFactory = brickFactory;
        _ballFactory = ballFactory;
        _projectileFactory = projectileFactory;
        Initialize();
    }

    /// <summary>
    /// Initializing sub nodes
    /// </summary>
    public override void _Ready()
	{
        _mainUI  = GetNode<MainUI>("MainUi");
        _barControl = GetNode<BarControl>("BarControl");
        _barStartMarker = GetNode<Marker2D>("BarStartPosition");
        _ballStartMarker = GetNode<Marker2D>("BallStartPosition");
        _trackerStartMarker = GetNode<Marker2D>("TrackerStartPosition");
        BossPath = GetNode<Path2D>("Path2D");
        BossPathFollow = GetNode<PathFollow2D>("Path2D/PathFollow2D");

        _barControl.HitByProjectile += OnBarHitByProjectile;
    }

    private async void OnBarHitByProjectile(int damage)
    {
        Lives += damage;
        _mainUI.UpdateLives(Lives);
        var isDead = await CheckDeath();
        if (isDead)
            StartGame();
    }

    #region Loading and starting level

    public override void _Process(double delta)
    {
        if (_boss != null)
        {
            var progress= BossPathFollow.Progress + (float)delta * _boss.Speed;
            BossPathFollow.Progress = progress;
            if (BossPathFollow.Progress < progress)
                LoadNextBossPath();
        }
    }

    /// <summary>
    /// Reset game
    /// </summary>
    public void Initialize()
    {
        InitializeLevelSeries();
        LoadAndStartGame();
    }

    private void LoadAndStartGame()
    {
        LoadLevel();
        StartGame();   
    }

    /// <summary>
    /// Initialize variables persisting through levels
    /// </summary>
    private void InitializeLevelSeries()
    {
        Lives = 3;
        Score = 0;
    }

    /// <summary>
    /// Load level
    /// </summary>
    private void LoadLevel()
    {
        var level = _levelService.GetLevel(_currentStage);
        BossPathFollow.Progress = 0;
        _numberOfBricks = level.Bricks.Count(b => b.BrickType != BrickType.Unbreakable);
        if (level.HasBoss)
        {
            _boss = PackedSceneLocator.GetScene<Boss>(level.BossName);
            _boss.HP = 10;
            _boss.Speed = 100;
            _boss.SetPaths(level.BossPaths);

            BossPathFollow.AddChild(_boss);
            LoadNextBossPath();

            _boss.BossHit += OnBossHit;
            _boss.BossDestroyed += OnBossDestroyed;
            _boss.BossSpawnProjectile += OnBossSpawnProjectile;
        }
        _mainUI.SwitchGameMode(level.HasBoss ? LevelObjective.Boss : LevelObjective.Bricks);

        foreach (var brickDto in level.Bricks)
        {
            var brick = _brickFactory.CreateBrick(brickDto);
            brick.OnBrickDestroyed += OnBrickDestroyed;
            CallDeferred(Node.MethodName.AddChild, brick);
            brick.Show();
        }
    }

    private void LoadNextBossPath()
    {
        BossPath.Curve = _boss.GetNextBossPath();
        BossPathFollow.Progress = 0;
    }

    private void OnBossSpawnProjectile()
    {
        var projectile = _projectileFactory.GetRandomProjectile();
        projectile.Position = (_boss.GlobalPosition - new Vector2(0, 10)) + _boss.ProjectileSpawnPoint;
        AddChild(projectile);
    }

    private void OnBossDestroyed()
    {
        Score += 1000;
        _boss.BossHit -= OnBossHit;
        _boss.BossDestroyed -= OnBossDestroyed;
        _boss.QueueFree();
        _boss = null;
        Lives++;
        LoadNextLevel();
    }

    private void OnBossHit(int hp)
    {
        _mainUI.UpdateRemainingBricks(hp);
    }

    /// <summary>
    /// Start game
    /// </summary>
    private void StartGame()
    {
        _mainUI.UpdateLives(Lives);
        if (_boss != null)
            _mainUI.UpdateRemainingBricks(_boss.HP);
        else
            _mainUI.UpdateRemainingBricks(_numberOfBricks);

        _mainUI.UpdateScore(Score);

        ClearDynamicallyCreatedItems();

        _barControl.Position = _barStartMarker.Position;
        _barControl.Show();

        _numberOfBallsCreated = 0;
        var ball = CreateBall(_ballStartMarker.Position);
        ball.IsAttachedToBar = true;
        ball.Show();

        _barControl.CanMove = true;
        CallDeferred(MethodName.AddChild, ball);
    }
    #endregion

    #region Level clear management
    /// <summary>
    /// Check if level is cleared on brick destroyed
    /// </summary>
    private void OnBrickDestroyed(int points)
    {
        _numberOfBricks--;
        _mainUI.BrickBroken();
        Score += points;
        _mainUI.UpdateScore(Score);
        _mainUI.UpdateRemainingBricks(_numberOfBricks);

        if (_numberOfBricks == 0)
            LoadNextLevel();
    }

    /// <summary>
    /// Load next level
    /// </summary>
    private async void LoadNextLevel()
    {
        StopParty();
        GetTree().CallGroup("UnbreakableBricks", Node.MethodName.QueueFree);

        _currentStage++;

        await _mainUI.ShowMessages(new List<TimedMessage>() { new TimedMessage("Level cleared !", 1.0f), new TimedMessage("Level " + _currentStage + " !", 1.0f)});

        LoadAndStartGame();
    }
    #endregion

    /// <summary>
    /// Wait while message is displayed (bad behaviour of wrong since we're doing it to wait on message display in MainUI while not truly having a dependance)
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    private async Task Wait(float time)
    {
        await ToSignal(GetTree().CreateTimer(time), SceneTreeTimer.SignalName.Timeout);
    }

    /// <summary>
    /// Stop party
    /// </summary>
    private void StopParty()
    {
        _barControl.SetPhysicsProcess(false);
        _barControl.Hide();

        Score += _balls.Count * 100;

        if (_boss != null)
            _boss.QueueFree();

        ClearDynamicallyCreatedItems();
        _balls.Clear();
        _bonusTrackers.Clear();
    }

    private void ClearDynamicallyCreatedItems()
    {
        GetTree().CallGroup("Projectiles", Node.MethodName.QueueFree);
        GetTree().CallGroup("balls", Node.MethodName.QueueFree);
        GetTree().CallGroup("BonusTrackers", Node.MethodName.QueueFree);
    }

    #region Ball management
    /// <summary>
    /// Create ball
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    private Ball CreateBall(Vector2 position)
    {
        var ball = _ballFactory.CreateBall(position);
        ball.ID = _numberOfBallsCreated++;
        ball.OnDuplicateBall += OnDuplicateBall;
        ball.OnHit += OnBallHit;
        _balls.Add(ball);
        GD.Print("Created ball: " + ball.ID);
        ball.Show();

        var bonusTracker = PackedSceneLocator.GetScene<BonusTracker>();
        bonusTracker.ID = ball.ID;
        bonusTracker.Scale *= (GD.Randf() + 0.5f);
        _bonusTrackers.Add(bonusTracker);
        AddChild(bonusTracker);
        bonusTracker.Position = _trackerStartMarker.Position;
        bonusTracker.ActivateLevel(0);
        ball.OnHit += bonusTracker.OnAssociatedBallHit;
        return ball;
    }

    private void OnBallHit(int ID, int intensity)
    {
        _mainUI.Hit();
    }

    /// <summary>
    /// Duplicate ball
    /// </summary>
    /// <param name="ball"></param>
    private void OnDuplicateBall(Ball ball)
    {
        GD.Print("Ball count: " + _balls.Count + " bricks count:" + _numberOfBricks);
        if (_balls.Count >= 10 || _numberOfBricks == 0)
            return;

        var duplicatedBall = CreateBall(ball.Position);
        duplicatedBall.CanMove = false;
        duplicatedBall.Rotation = ball.Rotation;
        GD.Randomize();
        var sign = GD.RandRange(0, 1) == 0 ? -1 : 1;
        var duplicatedVector = ball.LinearVelocity.Rotated(duplicatedBall.Rotation + sign * Mathf.Pi / 2);
        if (duplicatedVector.Y < 0.3 && duplicatedVector.Y > -0.3)
        {
            duplicatedVector = duplicatedVector.Rotated(duplicatedVector.Y > 0 ? Mathf.Pi / 6 : -Mathf.Pi / 6);
            if (duplicatedVector.X > 0)
                duplicatedVector.Y *= -1;
        }
        duplicatedBall.LinearVelocity = duplicatedVector;
        CallDeferred(MethodName.AddChild, duplicatedBall);
        duplicatedBall.Show();
    }

    /// <summary>
    /// On ball hit lose zone
    /// </summary>
    private async void OnBallHitLoseZone(int ID)
    {
        var ball = _balls.FirstOrDefault(b => b.ID == ID);

        var associatedBonusTracker = _bonusTrackers.FirstOrDefault(t => t.ID == ID);
        ball.OnHit -= associatedBonusTracker.OnAssociatedBallHit;
        _bonusTrackers.Remove(associatedBonusTracker);
        if (associatedBonusTracker != null)
            associatedBonusTracker.QueueFree();

        _balls.Remove(ball);

        if (_balls.Count != 0)
            return;

        Lives--;

        if (_boss != null && Lives >0)
        {
            GD.Print("QuickStartGame");
            StartGame();
            return;
        }

        await Wait(0.5f);

        _mainUI.UpdateLives(Lives);

        StopParty();

        await CheckDeath();
        StartGame();
    }

    private async Task<bool> CheckDeath()
    {
        if (Lives == 0)
        {
            _barControl.Hide();
            StopParty();
            EmitSignal(SignalName.OnFinalScore, Score);
            await _mainUI.ShowMessage("You lost :(");

            InitializeLevelSeries();
            LoadLevel();
            return true;
        }

        return false;
    }
    #endregion
}
