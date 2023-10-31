using Casse_brique.Domain.Configuration;
using Cassebrique.Locators;
using Godot;
using System.Collections.Generic;

public partial class SettingsScreen : Control
{
    private const string MenuButtonPath = "res://Scenes/UI/MainMenu/MenuButton.tscn";

    private VBoxContainer _settingsMenu;
	private PanelContainer _settingsPanel;
	private PackedScene _buttonPackedScene;

    private InputControls _inputControls;
    private SoundSettings _soundSettings;

    private Control _currentNode = null;

    [Signal]
    public delegate void OnRequestQuitEventHandler();

    private GameSettingsService _gameSettingsService;

    public void Setup(GameSettingsService gameBaseSettings)
    {
        _gameSettingsService = gameBaseSettings;
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		_settingsMenu = GetNode<VBoxContainer>("SettingsType");
		_settingsPanel = GetNode<PanelContainer>("Settings");
        _buttonPackedScene = ResourceLoader.Load<PackedScene>(MenuButtonPath);

        _inputControls = PackedSceneLocator.GetScene<InputControls>();
        _inputControls.Hide();
        _settingsPanel.AddChild(_inputControls);        

        _soundSettings = PackedSceneLocator.GetScene<SoundSettings>();
        _soundSettings.Setup(_gameSettingsService.GetSoundManager());
        _soundSettings.Hide();
        _settingsPanel.AddChild(_soundSettings);

		var buttons = CreateMenuButtons();
		foreach(var button in buttons)
			_settingsMenu.AddChild(button);

                
    }

	private List<MenuButton> CreateMenuButtons()
	{
		var buttons = new List<MenuButton>();

        var soundMenuButton = _buttonPackedScene.Instantiate<MenuButton>();
        soundMenuButton.Text = "Sound";
		soundMenuButton.Action = () => DisplayControls(_soundSettings);
		buttons.Add(soundMenuButton);

        var inputMenuButton = _buttonPackedScene.Instantiate<MenuButton>();
        inputMenuButton.Text = "Controls";
        inputMenuButton.Action = () => DisplayControls(_inputControls);
        buttons.Add(inputMenuButton);

        return buttons;
    }

    private void DisplayControls(Control selectedControl)
    {
        if (_currentNode != null)
            _currentNode.Hide();

        _currentNode = selectedControl;
        _currentNode.Show();
    }
}
