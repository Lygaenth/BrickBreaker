using Casse_brique.DAL;
using Casse_brique.DAL.API;
using Casse_brique.Domain.API;
using Casse_brique.Domain.Scoring;
using Casse_brique.Services;
using Cassebrique.Domain.API;
using Cassebrique.Factory;
using Cassebrique.Scenes.UI;
using Cassebrique.Services;
using Godot;
using static System.Formats.Asn1.AsnWriter;

public partial class Main : Node
{
    #region NodePaths
    private const string LevelPath = "res://Scenes/GamePlay/Level.tscn";
    private const string MainMenuPath = "res://Scenes/UI/MainMenu/MainMenu.tscn";
    private const string HighScorePath = "res://Scenes/UI/Scores/HighScores.tscn";
    private const string ControlsPath = "res://Scenes/UI/InputControls.tscn";
    private const string UserEntryPath = "res://Scenes/UI/Scores/UserEntry.tscn";
    #endregion

    #region Services
    private readonly ILevelService _levelService;
    private IHighScoreService _highScoreService;
    private IBrickFactory _brickFactory;
    private IProjectileFactory _projectileFactory;
    private IHighScoreDal _highScoreDal;
    #endregion

    private bool _gameIsOn = false;
    private ScreenType _currentScreen;
    private bool _paused = false;
    private int _currentScore;


    #region SubNodes
    private Level _level = null;
    private MainMenu _menu = null;
    private HighScores _highScores = null;
    private HttpRequest _httpRequest = null;
    private InputControls _inputScreen = null;
    private UserEntry _userEntry = null;
    #endregion

    private PackedScene _mainMenuPackedScene;
    private PackedScene _highScoresScreenPackedScene;
    private PackedScene _inputScreenPackedScene;

    /// <summary>
    /// Constructor
    /// </summary>
    public Main()
    {
        _levelService = new LevelService();
        _brickFactory = new BrickFactory();
        _projectileFactory = new ProjectileFactory();
        _mainMenuPackedScene = ResourceLoader.Load<PackedScene>(MainMenuPath);
        _highScoresScreenPackedScene = ResourceLoader.Load<PackedScene>(HighScorePath);
        _inputScreenPackedScene = ResourceLoader.Load<PackedScene>(ControlsPath);

        _highScoreDal = new HighScoreDal();
    }

    public override void _Ready()
    {
        bool isOnline = false;

        _httpRequest = GetNode<HttpRequest>("/root/HighScoreHttpRequest");
        _highScoreService = isOnline ? new OnlineHighScoreService(_httpRequest) : new LocalHighScoreService(_highScoreDal);
        GD.Print(OS.GetExecutablePath());
        LoadMenu();
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("DisplayMenu"))
        {
            if (_currentScreen == ScreenType.Scores)
            {
                if(_highScores.AllowNavigation)
                    OnQuitScoresRequested();
                return;
            }

            if (_currentScreen == ScreenType.Inputs)
            {
                OnQuitInputRequested();
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
        _highScores = _highScoresScreenPackedScene.Instantiate<HighScores>();
        _highScores.Setup(_highScoreService);
        AddChild(_highScores);
        _highScores.OnQuitScoresRequested += OnQuitScoresRequested;
        _highScores.UpdateScores();
        _currentScreen = ScreenType.Scores;
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
        _currentScreen = ScreenType.MainMenu;
    }

    private void OnRequestInputs()
    {
        _menu.QueueFree();
        _inputScreen = _inputScreenPackedScene.Instantiate<InputControls>();
        AddChild(_inputScreen);
        _currentScreen = ScreenType.Inputs;
        _inputScreen.OnRequestQuit += OnQuitInputRequested;
    }

    private void OnQuitInputRequested()
    {
        _inputScreen.OnRequestQuit -= OnQuitInputRequested;
        _inputScreen.QueueFree();
        LoadMenu();
        _currentScreen = ScreenType.MainMenu;
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
        _level.Setup(_levelService, _brickFactory, _projectileFactory);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="score"></param>
    private void OnFinalScoreReceived(int score)
    {
        var userEntryPackedScene = ResourceLoader.Load<PackedScene>(UserEntryPath);
        _userEntry = userEntryPackedScene.Instantiate<UserEntry>();
        _level.QueueFree();
        _gameIsOn = false;
        AddChild(_userEntry);
        _currentScore = score;
        _userEntry.UpdateScore(score);
        _userEntry.ValidateUserName += OnValidateUserName;
    }

    private void OnValidateUserName(string userName)
    {
        _highScoreService.PostScore(new Score(0, userName, _currentScore));
        _userEntry.QueueFree();
        LoadMenu() ;
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
        _menu.RequestControls += OnRequestInputs;
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
        _menu.RequestControls -= OnRequestInputs;
        _menu.QueueFree();
    }

    #endregion
}
