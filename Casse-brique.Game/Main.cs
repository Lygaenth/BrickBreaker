using Casse_brique.DAL;
using Casse_brique.DAL.API;
using Casse_brique.Domain.API;
using Casse_brique.Domain.Scoring;
using Casse_brique.Services;
using Cassebrique.Domain.API;
using Cassebrique.Factory;
using Cassebrique.Services;
using Godot;

public partial class Main : Node
{
    #region NodePaths
    private const string LevelPath = "res://Scenes/GamePlay/Level.tscn";
    private const string MainMenuPath = "res://Scenes/UI/MainMenu/MainMenu.tscn";
    private const string HighScorePath = "res://Scenes/UI/Scores/HighScores.tscn";
    #endregion

    #region Services
    private readonly ILevelService _levelService;
    private IHighScoreService _highScoreService;
    private IBrickFactory _brickFactory;
    private IHighScoreDal _highScoreDal;
    #endregion

    private bool _gameIsOn = false;
    private bool _isInScoreScreen = false;
    private bool _paused = false;

    #region SubNodes
    private Level _level = null;
    private MainMenu _menu = null;
    private HighScores _highScores = null;
    private HttpRequest _httpRequest = null;
    #endregion

    private PackedScene _mainMenuPackedScene; 

    /// <summary>
    /// Constructor
    /// </summary>
    public Main()
    {
        _levelService = new LevelService();
        _brickFactory = new BrickFactory();
        _mainMenuPackedScene = ResourceLoader.Load<PackedScene>(MainMenuPath);
        _highScoreDal = new HighScoreDal();
    }

    public override void _Ready()
    {
        _httpRequest = GetNode<HttpRequest>("/root/HighScoreHttpRequest");
        _highScoreService = new LocalHighScoreService(_highScoreDal); //new OnlineHighScoreService(_httpRequest);
        GD.Print(OS.GetExecutablePath());
        LoadMenu();
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("DisplayMenu"))
        {
            if (_isInScoreScreen)
            {
                if(_highScores.AllowNavigation)
                    OnQuitScoresRequested();
                return;
            }

            if (_gameIsOn)
            {
                if (!_paused)
                {
                    GetTree().Paused = true;
                    _paused = true;
                    LoadMenu();
                    return;
                }

                UnloadMenu();
                GetTree().Paused = false;
                _paused = false;
            }

        }
    }

    #region Actions on notifications
    /// <summary>
    /// On request starting or going back to game
    /// </summary>
    private void OnRequestStart()
    {
        UnloadMenu();
        
        _paused = false;

        if (!_gameIsOn)
        {
            LoadGame();
            _gameIsOn = true;
        }
        else
        {   
            GetTree().Paused = false;
        }
    }

    /// <summary>
    /// On request reset level
    /// </summary>
    private void OnRequestReset()
    {
        GetTree().Paused = false;
        _level.QueueFree();
        LoadGame();
        UnloadMenu();
        _paused = false;
    }

    /// <summary>
    /// On requesting High score display
    /// </summary>
    private void OnRequestHighScores()
    {        
        _menu.QueueFree();
        var mainMenuPackedScene = ResourceLoader.Load<PackedScene>(HighScorePath);
        _highScores = mainMenuPackedScene.Instantiate<HighScores>();
        _highScores.Setup(_highScoreService);
        AddChild(_highScores);
        _highScores.OnQuitScoresRequested += OnQuitScoresRequested;
        _highScores.UpdateScores();
        _isInScoreScreen = true;
    }

    /// <summary>
    /// On quitting scores requested
    /// </summary>
    private void OnQuitScoresRequested()
    {
        _highScores.Unsubscribe();
        _highScores.OnQuitScoresRequested -= OnQuitScoresRequested;
        _highScores.QueueFree();
        LoadMenu();
        _isInScoreScreen = false;
    }
    #endregion

    #region Load components
    /// <summary>
    /// Load game
    /// </summary>
    private void LoadGame()
    {
        var levelPackedScene = ResourceLoader.Load<PackedScene>(LevelPath);
        _level = levelPackedScene.Instantiate<Level>();
        _level.OnFinalScore += OnFinalScoreReceived;
        AddChild(_level);
        _level.Setup(_levelService, _brickFactory);
    }

    private void OnFinalScoreReceived(int score)
    {
        GD.Print("Saving score");
        _highScoreService.PostScore(new Score(1, "User", score));
    }

    /// <summary>
    /// Load main menu
    /// </summary>
    private void LoadMenu()
    {
        _menu = _mainMenuPackedScene.Instantiate<MainMenu>();
        _menu.Setup(_gameIsOn);
        _menu.RequestStart += OnRequestStart;
        _menu.RequestRestart += OnRequestReset;
        _menu.RequestHighScore += OnRequestHighScores;
        AddChild(_menu);
    }

    /// <summary>
    /// Unload menu
    /// </summary>
    private void UnloadMenu()
    {
        if (_menu == null)
            return;

        _menu.RequestStart -= OnRequestStart;
        _menu.RequestRestart -= OnRequestReset;
        _menu.RequestHighScore -= OnRequestHighScores;
        _menu.QueueFree();
    }

    #endregion
}
