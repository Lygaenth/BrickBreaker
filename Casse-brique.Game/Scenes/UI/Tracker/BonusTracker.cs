using Godot;
using System;

public partial class BonusTracker : Node2D
{
	private BallBonus[] _bonuses;

	public int ID { get; set; }

	public BonusTracker()
	{
		_bonuses = new BallBonus[5];
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_bonuses[0] = GetNode<BallBonus>("Bonus1");
        _bonuses[1] = GetNode<BallBonus>("Bonus2");
        _bonuses[2] = GetNode<BallBonus>("Bonus3");
        _bonuses[3] = GetNode<BallBonus>("Bonus4");
        _bonuses[4] = GetNode<BallBonus>("Bonus5");
		ActivateLevel(0);
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="ID"></param>
	/// <param name="level"></param>
	public void OnAssociatedBallHit(int ID, int level)
	{
		ActivateLevel(level);
	}

	/// <summary>
	/// Activate bonus level
	/// </summary>
	/// <param name="level"></param>
	public void ActivateLevel(int level)
	{
		if (level > 1)
		{
			for (int i = 0; i < level - 1; i++)
				_bonuses[i].Overload();
		}

		if (level >= 1)
			_bonuses[level-1].Activate();
		
		if (level < _bonuses.Length - 1)
			for(int i = level; i < _bonuses.Length; i++)
				_bonuses[i].Deactivate();

		if (level == 0)
			GlobalPosition = new Vector2(GD.RandRange(0, 800) +100, Position.Y + GD.RandRange(-50,50));
	}
}
