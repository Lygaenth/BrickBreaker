using Godot;

public partial class UserEntry : Control
{
	[Signal]
	public delegate void ValidateUserNameEventHandler(string score);

	private TextEdit _userNameTextBox;
	private Label _scoreLabel;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_scoreLabel = GetNode<Label>("ScoreLabel");
		_userNameTextBox = GetNode<TextEdit>("TextEdit");
	}

	public void UpdateScore(int score)
	{
		_scoreLabel.Text = score.ToString();
	}

	private void OnButtonPressed()
	{
		EmitSignal(SignalName.ValidateUserName, _userNameTextBox.Text);
	}
}
