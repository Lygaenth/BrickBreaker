using Cassebrique.Scenes.UI.MainMenu;
using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class MainMenu : PanelContainer
{

    private const string MenuButtonPath = "res://Scenes/UI/MainMenu/MenuButton.tscn";

    private List<MenuButton> _buttonList;
    private VBoxContainer _buttonsPanel;
    private PackedScene _buttonPackedScene;
    private AudioStreamPlayer _soundEffect;

    [Signal]
	public delegate void RequestStartEventHandler();

    [Signal]
    public delegate void RequestRestartEventHandler();

    [Signal]
    public delegate void RequestHighScoreEventHandler();

    [Signal]
    public delegate void RequestControlsEventHandler();

    float[] _pitchScale = new[] { 0.1f, 0.2f, 0.5f, 1, 0.7f };
    int _pitchscaleIndex = 0;

    private MenuButton _selectedButton;

    public MainMenu()
    {
        _buttonPackedScene = ResourceLoader.Load<PackedScene>(MenuButtonPath);
        _buttonList = new List<MenuButton>();
    }

    public void Setup(bool isGameOn)
    {
        _buttonList = CreateButtons(isGameOn);
    }

    public override void _Ready()
    {
        _buttonsPanel = GetNode<VBoxContainer>("StartElements/ButtonsPanel");
        _soundEffect = GetNode<AudioStreamPlayer>("MenuSoundEffect");

        foreach (var button in _buttonList)
        {
            _buttonsPanel.AddChild(button);
            button.OnButtonPressed += OnButtonPressed;
        }
        SelectButton(0);
        foreach (var button in _buttonList)
            button.FocusEntered += OnButtonFocused;
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionPressed("Enter"))
        {
            OnButtonPressed(_buttonList.FirstOrDefault(b => b.HasFocus()).Action.GetHashCode());
        }
    }

    private void SelectButton(int index)
    {
        if (index < 0 || index >= _buttonList.Count)
            return;
        _selectedButton = _buttonList[index];
        GD.Print(_selectedButton.Action);
        _selectedButton.GrabFocus();
    }

    private List<MenuButton> CreateButtons(bool isGameOn)
    {
        var buttons = new List<MenuButton>();
        // Start/continue button
        var startButton = _buttonPackedScene.Instantiate<MenuButton>();
        startButton.Action = MenuAction.Start;
        startButton.Text = isGameOn ? "Continue" : "Start";
        buttons.Add(startButton);

        // Reset button
        if (isGameOn)
        {
            var resetButton = _buttonPackedScene.Instantiate<MenuButton>();
            resetButton.Action = MenuAction.Reset;
            resetButton.Text = "Reset";
            buttons.Add(resetButton);
        }

        // High scores button
        var highScoreButton = _buttonPackedScene.Instantiate<MenuButton>();
        highScoreButton.Action = MenuAction.HighScore;
        highScoreButton.Text = "Scores";
        buttons.Add(highScoreButton);

        // Control button
        var controlsButton = _buttonPackedScene.Instantiate<MenuButton>();
        controlsButton.Action = MenuAction.Controls;
        controlsButton.Text = "Controls";
        buttons.Add(controlsButton);

        // Quit button
        var quitButton = _buttonPackedScene.Instantiate<MenuButton>();
        quitButton.Action = MenuAction.Quit;
        quitButton.Text = "Quit";
        buttons.Add(quitButton);

        GD.Print("Created "+buttons.Count+" buttons");

        return buttons;
    }

    private void OnButtonFocused()
    {
        _soundEffect.PitchScale = _pitchScale[_pitchscaleIndex];
        _soundEffect.Play();
        _pitchscaleIndex++;
        if (_pitchscaleIndex >= _pitchScale.Length)
            _pitchscaleIndex = 0;
    }

    private void OnButtonPressed(int actionCode)
    {
        var action = (MenuAction)actionCode;
        switch(action)
        {
            case MenuAction.Start:
            case MenuAction.Continue:
                OnStartButtonPressed();
                break;
            case MenuAction.Reset:
                OnRestartButtonPressed();
                break;
            case MenuAction.HighScore:
                OnScoreButtonPressed();
                break;
            case MenuAction.Controls:
                OnControlsButtonPressed();
                break;
            case MenuAction.Quit:
                GetTree().Quit();
                break;
        }
    }

    private void OnStartButtonPressed()
	{
		EmitSignal(SignalName.RequestStart);
	}

    private void OnRestartButtonPressed()
    {
        EmitSignal(SignalName.RequestRestart);
    }

    private void OnScoreButtonPressed()
	{
		EmitSignal(SignalName.RequestHighScore);
	}

    private void OnControlsButtonPressed()
    {
        EmitSignal(SignalName.RequestControls);
    }
}
