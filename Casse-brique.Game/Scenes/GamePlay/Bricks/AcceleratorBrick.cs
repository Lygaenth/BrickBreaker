using Godot;
using System;

public partial class AcceleratorBrick : Brick
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    protected override void OnBrickHit(Ball ball, Vector2 velocity)
    {
        ball.Bounce(IsBrickHeavy, 5);
        ApplyBounceVelocity(ball, velocity, ball.Speed);

        HP--;

        if (HP <= 0)
            RaiseBroken(ball);
    }
}
