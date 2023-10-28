using Casse_brique.Domain.Enums;
using Godot;

public partial class Projectile : RigidBody2D
{
	private int _speed = 300;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var colliders = GetCollidingBodies();
		if (colliders.Count > 0)
		{
			if (colliders[0] is Ball ball)
				ball.Bounce(true, 1, AxisBounce.XY, Vector2.Zero);
				//ball.UpdateVelocity(((GlobalPosition - ball.GlobalPosition).Normalized() - ball.LinearVelocity.Normalized()) * ball.Speed,);
			QueueFree();
			return;
		}

		GlobalPosition = new Vector2(GlobalPosition.X, GlobalPosition.Y + (float)delta * _speed);
	}
}
