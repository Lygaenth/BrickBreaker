using Casse_brique.Domain.API;
using Casse_brique.Domain.Scoring;
using Cassebrique.Exceptions;
using Godot;
using System.Collections.Generic;

/// <summary>
/// High scores nodes
/// </summary>
public partial class HighScores : Control
{
    #region SubNodes
    private Container _scorePanel;
	private Label _errorMessage;
	private Timer _resultTimer;
    #endregion

    [Signal]
	public delegate void OnQuitScoresRequestedEventHandler();

	public bool AllowNavigation { get; set; }

	private IHighScoreService _scoreService;

	private PackedScene _scoreScene;

	private bool _resultsReceived;

    #region Construction/Initialization
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		_scorePanel = GetNode<Container>("ScoresPanel");
		_resultTimer = GetNode<Timer>("ResultTimer");
		_errorMessage = GetNode<Label>("ConnexionErrorLabel");
        _scoreScene = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Scores/UserScore.tscn");
    }

	/// <summary>
	/// Setup service since service cannot be passed through constructor
	/// </summary>
	/// <param name="highScoreService"></param>
	public void Setup(IHighScoreService highScoreService)
	{
		_scoreService = highScoreService;
        _scoreService.OnHighScoreResult += OnHighScoreResultReceived;
	}
	#endregion

	/// <summary>
	/// Unsubscribe (must be called because queue freeing the node doesn't remove the connection)
	/// </summary>
	public void Unsubscribe()
	{
        _scoreService.OnHighScoreResult -= OnHighScoreResultReceived;
    }

	/// <summary>
	/// On High scores results received from server
	/// </summary>
	/// <param name="scores"></param>
    private void OnHighScoreResultReceived(List<Score> scores)
    {
		_resultsReceived = true;
		int offset = 0;
        foreach (Score score in scores)
        {
            GD.Print(score);
            var userScore = _scoreScene.Instantiate<UserScore>();
            _scorePanel.AddChild(userScore);
			userScore.GlobalPosition = new Vector2(0, 150 + offset);
            userScore.Update(score);
			offset += 40;
        }
		AllowNavigation = true;
    }

	/// <summary>
	/// UpdateScore
	/// </summary>
	public void UpdateScores()
	{
		try
		{
			AllowNavigation = false;
			_resultsReceived = false;
            _resultTimer.Timeout += OnResultTimeout;
			_resultTimer.Start();
			_scoreService.RequestHighScores();
		}
		catch(ConnectionFailedException e)
		{
			DisplayError(e.Error);
		}
	}

	/// <summary>
	/// On result not received from server
	/// </summary>
    private void OnResultTimeout()
    {
		if (!_resultsReceived)
			DisplayError(Error.Ok);
		AllowNavigation = true;
    }

	/// <summary>
	/// Display error
	/// </summary>
	/// <param name="error"></param>
	private void DisplayError(Error error)
	{
        _errorMessage.Text = "Failed to get data from serveur." + (error != Error.Ok ? " Error code: " + error : "")+" :(";
        _errorMessage.Show();
    }

	/// <summary>
	/// On quit High error screen
	/// </summary>
    public void OnQuitButtonPressed()
	{
		if (AllowNavigation)
			EmitSignal(SignalName.OnQuitScoresRequested);
	}
}
