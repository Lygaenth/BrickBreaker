using Casse_brique.DAL;
using Casse_brique.DAL.API;
using Casse_brique.Domain.API;
using Casse_brique.Domain.Level;
using Casse_brique.Domain.Scoring;
using Casse_brique.Services;
using Cassebrique.Domain.API;
using Cassebrique.Enums;
using Cassebrique.Factory;
using Cassebrique.Locators;
using Cassebrique.Scenes.UI;
using Cassebrique.Services;
using Godot;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

public partial class Main : Node
{
    #region Services
    private IHighScoreService _highScoreService;
    private readonly ServiceProvider _serviceProvider;
    private IConfigurationRoot _configuration;
    #endregion

    private bool _gameIsOn = false;
    private ScreenType _currentScreen;
    private bool _paused = false;
    private int _currentScore;


    #region SubNodes
    private Level _level = null;
    private MainMenu _menu = null;
    private HighScores _highScores = null;
    //private HttpRequest _httpRequest = null;
    private InputControls _inputScreen = null;
    private UserEntry _userEntry = null;
    #endregion

    /// <summary>
    /// Constructor
    /// </summary>
    public Main()
    {
        CreateConfiguration();
        _serviceProvider = RegisterServices();
        RegisterScenes();
    }

    private void CreateConfiguration()
    {
        var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        _configuration = builder.Build();
    }

    private ServiceProvider RegisterServices()
    {
        GD.Print("Registering services");
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddTransient<ILevelDal, LevelDal>();
        serviceCollection.AddTransient<IHighScoreDal, HighScoreDal>();
        serviceCollection.AddTransient<ILevelService, LevelService>();
        serviceCollection.AddTransient<IBrickFactory, BrickFactory>();
        serviceCollection.AddTransient<IBallFactory, BallFactory>();
        serviceCollection.AddTransient<IBossFactory, BossFactory>();
        serviceCollection.AddTransient<IAuthenticationTokenService, AuthenticationTokenService>();
        serviceCollection.AddTransient<IProjectileFactory, ProjectileFactory>();
        serviceCollection.AddSingleton<IAutoLoaderProvider, AutoLoaderProvider>();

        if (_configuration["OnlineActive"] == "true")
        {
            GD.Print("Registering online highscore service");
            serviceCollection.AddTransient<IHighScoreService, OnlineHighScoreService>();
        }
        else
        {
            GD.Print("Registering local highscore service");
            serviceCollection.AddTransient<IHighScoreService, LocalHighScoreService>();
        }

        return serviceCollection.BuildServiceProvider();
    }

    private void RegisterScenes()
    {
        PackedSceneLocator.Register<Ball>("res://Scenes/GamePlay/Ball.tscn");
        PackedSceneLocator.Register<Level>("res://Scenes/GamePlay/Level.tscn");
        PackedSceneLocator.Register<MainMenu>("res://Scenes/UI/MainMenu/MainMenu.tscn");
        PackedSceneLocator.Register<HighScores>("res://Scenes/UI/Scores/HighScores.tscn");
        PackedSceneLocator.Register<InputControls>("res://Scenes/UI/InputControls.tscn");
        PackedSceneLocator.Register<InputControls>("res://Scenes/UI/Scores/UserEntry.tscn");
        PackedSceneLocator.Register<BonusTracker>("res://Scenes/UI/Tracker/BonusTracker.tscn");
        PackedSceneLocator.Register<UserEntry>("res://Scenes/UI/Scores/UserEntry.tscn");

        PackedSceneLocator.Register<Projectile>("res://Scenes/GamePlay/Projectiles/Violon.tscn", ProjectileTypes.Violon.GetHashCode().ToString());
        PackedSceneLocator.Register<Projectile>("res://Scenes/GamePlay/Projectiles/Piano.tscn", ProjectileTypes.Piano.GetHashCode().ToString());
        PackedSceneLocator.Register<Projectile>("res://Scenes/GamePlay/Projectiles/Clarinette.tscn", ProjectileTypes.Clarinette.GetHashCode().ToString());
    }

    public override void _Ready()
    {
        var autoloaderProvider = _serviceProvider.GetService<IAutoLoaderProvider>();
        autoloaderProvider.Initialize(this);
        _highScoreService = _serviceProvider.GetService<IHighScoreService>();
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
        _highScores = PackedSceneLocator.GetScene<HighScores>();
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
        _inputScreen = PackedSceneLocator.GetScene<InputControls>();
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
        _level = PackedSceneLocator.GetScene<Level>();
        _level.OnFinalScore += OnFinalScoreReceived;
        AddChild(_level);

        var level = new LevelModel(_serviceProvider.GetService<ILevelService>());

        _level.Setup(level,
            _serviceProvider.GetService<IBrickFactory>(),
            _serviceProvider.GetService<IBallFactory>(),
            _serviceProvider.GetService<IBossFactory>(),
            _serviceProvider.GetService<IProjectileFactory>());

        level.LoadLevel();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="score"></param>
    private void OnFinalScoreReceived(int score)
    {
        _userEntry = PackedSceneLocator.GetScene<UserEntry>();
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
        _menu = PackedSceneLocator.GetScene<MainMenu>();
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
