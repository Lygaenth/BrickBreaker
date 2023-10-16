using Cassebrique;

public partial class UnbreakableBrick : Brick
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
        ball.Bounce(IsBrickHeavy, -2, axisBounce);
    }
}
