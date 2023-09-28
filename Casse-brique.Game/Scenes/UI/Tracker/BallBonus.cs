using Godot;
using System;

public partial class BallBonus : AnimatedSprite2D
{
	public bool IsActive { get; private set; }

	// This classs holds logic for bonus display this should go through these steps: deactivated, activated, overloaded (when bonus is higher)
	public void Activate()
	{
		if (!IsActive)
			this.Scale = Scale * (float)GD.RandRange(1.0f, 1.5f);

		IsActive = true;
		this.Show();
		this.Play();
		this.Animation = "Active";
	}

	public void Overload()
	{
		this.Animation = "Inactive";
	}

	public void Deactivate()
	{
		Scale = new Vector2(0.25f, 0.25f);
		this.Hide();
	}
}
