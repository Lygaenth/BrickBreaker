using Cassebrique.Domain.API;
using Cassebrique.Domain.Bricks;
using Cassebrique.Factory;
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
    #endregion

    [Signal]
    public delegate void OnFinalScoreEventHandler(int score);

    [Export]
    public PackedScene BallScene { get; set; }

    [Export]
    public PackedScene BrickScene { get; set; }

    [Export]
    public int Lives { get; set; }

    public int Score { get; set; }

    private int _numberOfBricks = 0;
    private int _numberOfBalls = 0;
    private int _currentLevel = 1;

    private IBrickFactory _brickFactory;
    private ILevelService _levelService;

    /// <summary>
    /// Initializing values since it cannot be passed through constructor
    /// </summary>
    /// <param name="levelService"></param>
    /// <param name="brickFactory"></param>
    public void Setup(ILevelService levelService, IBrickFactory brickFactory)
    {
        _levelService = levelService;
        _brickFactory = brickFactory;
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
    }

    #region Loading and starting level

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
        var bricks = _levelService.GetBricks(_currentLevel);
        _numberOfBricks = bricks.Count(b => b.BrickType != BrickType.Unbreakable);
        foreach (var brickDto in bricks)
        {
            var brick = _brickFactory.CreateBrick(brickDto);
            brick.OnBrickDestroyed += OnBrickDestroyed;
            CallDeferred(Node.MethodName.AddChild, brick);
            brick.Show();
        }
    }

    /// <summary>
    /// Start game
    /// </summary>
    private void StartGame()
    {
        _mainUI.UpdateLives(Lives);
        _mainUI.UpdateRemainingBricks(_numberOfBricks);
        _mainUI.UpdateScore(Score);

        _barControl.Position = _barStartMarker.Position;
        _barControl.Show();

        var ball = CreateBall(_ballStartMarker.Position);
        ball.IsAttachedToBar = true;
        ball.Show();

        _numberOfBalls = 1;
        _barControl.CanMove = true;

        CallDeferred(Node.MethodName.AddChild, ball);
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

        _currentLevel++;

        await _mainUI.ShowMessages(new List<TimedMessage>() { new TimedMessage("Level cleared !", 1.0f), new TimedMessage("Level " + _currentLevel + " !", 1.0f)});

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

        GetTree().CallGroup("balls", Node.MethodName.QueueFree);
    }

    #region Ball management
    /// <summary>
    /// Create ball
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    private Ball CreateBall(Vector2 position)
    {
        var ball = BallScene.Instantiate<Ball>();
        ball.Scale = new Vector2(0.3f, 0.3f);
        ball.Position = position;
        ball.OnDuplicateBall += OnDuplicateBall;
        ball.OnHit += OnBallHit;
        ball.Show();
        _numberOfBalls++;
        return ball;
    }

    private void OnBallHit()
    {
        _mainUI.Hit();
    }

    /// <summary>
    /// Duplicate ball
    /// </summary>
    /// <param name="ball"></param>
    private void OnDuplicateBall(Ball ball)
    {
        if (_numberOfBalls >= 10)
            return;

        var duplicatedBall = CreateBall(ball.Position);
        duplicatedBall.CanMove = false;
        duplicatedBall.Rotation = ball.Rotation;
        GD.Randomize();
        var sign = GD.RandRange(0, 1) == 0 ? -1 : 1;
        duplicatedBall.LinearVelocity = ball.LinearVelocity.Rotated(duplicatedBall.Rotation + sign * Mathf.Pi / 4);

        CallDeferred(MethodName.AddChild, duplicatedBall);
        duplicatedBall.Show();
    }

    /// <summary>
    /// On ball hit lose zone
    /// </summary>
    private async void OnBallHitLoseZone()
    {
        _numberOfBalls--;
        if (_numberOfBalls != 0 || _numberOfBricks == 0)
            return;

        await Wait(0.5f);

        StopParty();

        Lives--;
        _mainUI.UpdateLives(Lives);

        if (Lives == 0)
        {
            EmitSignal(SignalName.OnFinalScore, Score);
            await _mainUI.ShowMessage("You lost :(");

            InitializeLevelSeries();
            LoadLevel();   
        }

        GD.Print("Starting game");
        StartGame();
    }
    #endregion
}
