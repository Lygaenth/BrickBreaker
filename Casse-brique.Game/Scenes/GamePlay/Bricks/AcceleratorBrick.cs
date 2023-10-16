using Cassebrique;
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

    protected override void OnBrickHit(Ball ball, AxisBounce axisBounce)
    {
        ball.Bounce(IsBrickHeavy, 5, axisBounce);

        HP--;

        if (HP <= 0)
            RaiseBroken(ball);
    }
}
