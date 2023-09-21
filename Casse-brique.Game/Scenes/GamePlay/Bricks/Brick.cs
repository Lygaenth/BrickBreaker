using Cassebrique.Domain.Bricks;
using Godot;

public partial class Brick : Node2D
{
	[Export]
	public int HP { get; set; } = 1;

	[Export]
	public BrickType BrickType { get; set; }

	[Export]
    public int Points { get; set; } = 10;

	[Export]
    public bool IsDivider { get; set; }

	[Export]
    public bool IsAccelerator { get; set; }

	[Export]
	public bool IsBrickHeavy { get; set; }

    [Signal]
	public delegate void OnBrickDestroyedEventHandler(int point);



	private bool _hasDuplicated = false;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    private void OnTopOrBottomAreaEntered(Node2D body)
	{
        if (body is not Ball ball)
            return;

        var velocity = ball.LinearVelocity;
		velocity.Y *= -1;

		OnBrickHit(ball, velocity);
    }

	private void OnLeftOrRightAreaEntered(Node2D body)
	{
        if (body is not Ball ball)
            return;

        var velocity = ball.LinearVelocity;
        velocity.X *= -1;

        OnBrickHit(ball, velocity);
    }

    private void OnBrickHit(Ball ball, Vector2 velocity)
	{		
		ball.Bounce(IsBrickHeavy);
		ball.LinearVelocity = velocity.Normalized() * (IsAccelerator ? 2 : 1) * ball.Speed;

		if (IsDivider && !_hasDuplicated)
		{
			_hasDuplicated = true;
			ball.MakeDuplicate();
		}

		HP--;
		if (HP == 0)
		{
			EmitSignal(SignalName.OnBrickDestroyed, Points*ball.Bonus);
            this.QueueFree();
		}
	}
}
