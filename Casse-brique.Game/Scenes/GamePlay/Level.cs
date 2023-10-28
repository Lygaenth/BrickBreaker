using Casse_brique.Domain.Enums;
using Casse_brique.Domain.Level;
using Cassebrique.Factory;
using Cassebrique.Locators;
using Cassebrique.Scenes.UI;
using Godot;
using System.Collections.Generic;
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

    private Boss _boss;

    private readonly List<Ball> _balls;
    private readonly List<BonusTracker> _bonusTrackers;

    private IBrickFactory _brickFactory;
    private IBallFactory _ballFactory;
    private IBossFactory _bossFactory;
    private IProjectileFactory _projectileFactory;
    private LevelModel _levelModel;

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
    public void Setup(LevelModel levelModel, IBrickFactory brickFactory, IBallFactory ballFactory, IBossFactory bossFactory, IProjectileFactory projectileFactory)
    {
        _brickFactory = brickFactory;
        _bossFactory = bossFactory;
        _ballFactory = ballFactory;
        _projectileFactory = projectileFactory;

        _levelModel = levelModel;
        _levelModel.GameStateUpdated += OnScoreUpdated;
        _levelModel.LevelLoaded += OnLevelLoaded;
        _levelModel.LevelEnded += OnLevelEnded;
        _levelModel.PlayerLost += OnPlayerLost;
        _levelModel.OnAnyBounce += OnAnyBallBounce;
        _levelModel.OnBallCreated += OnBallCreated;
        _levelModel.ResetBarPosition += OnResetBarPosition;
    }

    private void OnResetBarPosition(object sender, System.EventArgs e)
    {
        ResetBar();
    }

    private void OnAnyBallBounce(object sender, System.EventArgs e)
    {
        _mainUI.Hit();
    }

    private void OnBallCreated(object sender, BallCreationInfo e)
    {
        CreateBall(e);
    }

    private void OnLevelEnded(object sender, System.EventArgs e)
    {
        StopParty();
    }

    private void OnLevelLoaded(object sender, LevelObjective levelObjective)
    {
        LoadLevel(levelObjective);
    }

    private void OnScoreUpdated(object sender, GameState scoreUpdate)
    {
        if (scoreUpdate.BrickWasBroke)
            _mainUI.BrickBroken();
    
        _mainUI.UpdateRemainingBricks(scoreUpdate.NumberOfRemainingBricks);
        _mainUI.UpdateScore(scoreUpdate.Score);
        _mainUI.UpdateLives(scoreUpdate.Lives);
    }

    private async void OnPlayerLost(object sender, int finalScore)
    {
        GD.Print("Player lost");
        StopParty();
        EmitSignal(SignalName.OnFinalScore, finalScore);
        await _mainUI.ShowMessage("You lost :(");
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

    private void OnBarHitByProjectile(int damage)
    {
        _levelModel.Damage(damage);
    }

    #region Loading and starting level

    /// <summary>
    /// Load level
    /// </summary>
    private void LoadLevel(LevelObjective levelObjective)
    {
        BossPathFollow.Progress = 0;

        if (_levelModel.Boss != null)
        {
            _boss = _bossFactory.CreateBoss(_levelModel.Boss);
            BossPathFollow.AddChild(_boss);

            _boss.BossSpawnProjectile += OnBossSpawnProjectile;
        }

        foreach (var brickDto in _levelModel.Bricks)
            AddDefered(_brickFactory.CreateBrick(brickDto));

        _mainUI.SwitchGameMode(levelObjective);
        ResetBar();
    }

    private void OnBossSpawnProjectile()
    {
        var projectile = _projectileFactory.GetRandomProjectile();
        projectile.Position = (_boss.GlobalPosition - new Vector2(0, 10)) + _boss.ProjectileSpawnPoint;
        AddChild(projectile);
    }

    private void AddDefered(Node2D node)
    {
        CallDeferred(MethodName.AddChild, node);
    }

    #endregion

    #region Level clear management
    /// <summary>
    /// Check if level is cleared on brick destroyed
    /// </summary>
    private void OnBrickDestroyed()
    {
        _mainUI.BrickBroken();
    }

    /// <summary>
    /// Load next level
    /// </summary>
    private async void LoadNextLevel()
    {
        StopParty();
        GetTree().CallGroup("UnbreakableBricks", Node.MethodName.QueueFree);

        await _mainUI.ShowMessages(new List<TimedMessage>() { new TimedMessage("Level cleared !", 1.0f), new TimedMessage("Level " + _levelModel.CurrentStage + " !", 1.0f)});
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

        if (_boss != null)
            _boss.QueueFree();

        ClearDynamicallyCreatedItems();
        _balls.Clear();
        _bonusTrackers.Clear();
    }

    private void ResetBar()
    {
        _barControl.Position = _barStartMarker.Position;
        _barControl.Show();
        _barControl.CanMove = true;
    }

    /// <summary>
    /// Clear items
    /// </summary>
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
    private Ball CreateBall(BallCreationInfo creationInfo)
    {
        var ballPosition = creationInfo.Position;
        if (ballPosition.X == 0 && ballPosition.Y == 0)
            ballPosition = GetNode<Marker2D>("BallStartPosition").GlobalPosition;

        var ballModel = _levelModel.Balls.First(b => b.ID == creationInfo.ID);
        var ball = _ballFactory.CreateBall(ballModel, ballPosition);
        _balls.Add(ball);
        AddDefered(ball);

        ball.LinearVelocity = creationInfo.InitialVelocity;
        ball.Show();

        var bonusTracker = PackedSceneLocator.GetScene<BonusTracker>();
        bonusTracker.Setup(ballModel);
        bonusTracker.Scale *= (GD.Randf() + 0.5f);
        _bonusTrackers.Add(bonusTracker);
        AddChild(bonusTracker);
        bonusTracker.Position = _trackerStartMarker.Position;        
        bonusTracker.ActivateLevel(0);

        return ball;
    }
    #endregion
}
