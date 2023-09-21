using Godot;

/// <summary>
/// Game UI during play 
/// </summary>
public partial class MainUI : Control
{
    #region SubNodes
    private Timer _messageTimer;
	private Label _messageLabel;
	private Label _livesLabel;
	private Label _brickLabel;
    private Label _scoreLabel;
    private AnimatedSprite2D _liveSprite;
	private Timer _iconTimer;
	private AnimatedSprite2D _brickSprite;
    #endregion

    /// <summary>
    /// Initializing node on ready
    /// </summary>
    public override void _Ready()
	{
		_messageLabel = GetNode<Label>("MessageLabel");
        _messageTimer = GetNode<Timer>("MessageTimer");
		_livesLabel = GetNode<Label>("LivesLabel");
		_liveSprite = GetNode<AnimatedSprite2D>("LiveIcon");
		_iconTimer = GetNode<Timer>("LiveIcon/LiveIconTimer");
		_brickLabel = GetNode<Label>("BrickLabel");
		_scoreLabel = GetNode<Label>("ScoreLabel");
        _brickSprite = GetNode<AnimatedSprite2D>("BrickIcon");

        _liveSprite.Play();
    }

    #region Display message
	/// <summary>
	/// Display message for 2 secondes
	/// </summary>
	/// <param name="message"></param>
    public void ShowMessage(string message)
	{
		_messageLabel.SetDeferred(Label.PropertyName.Text, message);
		_messageLabel.Show();
		_messageTimer.Start();
	}

    /// <summary>
    /// On time out hide message
    /// </summary>
    private void OnMessageTimerTimeout()
    {
        _messageLabel.Hide();
    }
	#endregion

	/// <summary>
	/// Update lives
	/// </summary>
	/// <param name="lives"></param>
	public void UpdateLives(int lives)
	{
		_livesLabel.Text = lives.ToString();
	}

	/// <summary>
	/// Updates remaining bricks
	/// </summary>
	/// <param name="bricks"></param>
	public void UpdateRemainingBricks(int bricks)
	{
		_brickLabel.Text = bricks.ToString();
	}

	/// <summary>
	/// Update score
	/// </summary>
	/// <param name="points"></param>
	public void UpdateScore(int points)
	{
		_scoreLabel.Text = points.ToString();
	}

	public void Hit()
	{
		_liveSprite.Animation = "Hit";
        _liveSprite.Play();
        _iconTimer.Start();
	}

	public void OnLiveIconTimeout()
	{
        _liveSprite.Animation = "Still";
		_liveSprite.Play();
    }

    public void BrickBroken()
	{
		_brickSprite.Animation = "Shake";
		_brickSprite.Play();
	}

    private void OnBrickShakeFinished()
    {
		if (_brickSprite.Animation == "Shake")
	        _brickSprite.Animation = "Still";
    }
}
