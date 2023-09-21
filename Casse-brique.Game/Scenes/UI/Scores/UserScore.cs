using Casse_brique.Domain.Scoring;
using Godot;
using System;

/// <summary>
/// Node for individual user score
/// </summary>
public partial class UserScore : Control
{
    #region SubNodes
    private Label _rankLabel;
	private Label _userNameLabel;
	private Label _userScoreLabel;
    #endregion

    public override void _Ready()
	{
		_rankLabel = GetNode<Label>("RankLabel");
		_userNameLabel = GetNode<Label>("UserNameLabel");
		_userScoreLabel = GetNode<Label>("UserScoreLabel");
	}

	/// <summary>
	/// Update values
	/// </summary>
	/// <param name="score"></param>
	public void Update(Score score)
	{
		_userNameLabel.Text = score.UserName;
        _rankLabel.Text = score.Rank.ToString();
        _userScoreLabel.Text = score.Points.ToString();
	}
}
