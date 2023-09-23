using Cassebrique.Domain.Bricks;
using Godot;
using System;

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

	private DateTime _lastHitTime;
	private bool _hasDuplicated = false;
	
	Brick()
	{
		_lastHitTime = DateTime.Now;
	}

	private bool CheckIsValid(Node2D body, out Ball ball)
	{
        var hitTime = DateTime.Now;
		if (body is not Ball)
		{
			ball = null;
			return false;
		}

		if ((hitTime - _lastHitTime).Milliseconds < 10)
		{
			ball = null;
			return false;
		}

        _lastHitTime = hitTime;
        ball = body as Ball;
		return true;
    }

    private void OnTopAreaEntered(Node2D body)
	{
		Ball ball;
		if (!CheckIsValid(body, out ball))
			return;

        var velocity = ball.LinearVelocity;
        if (velocity.Y > 0)
        {
            velocity.Y *= -1;
            OnBrickHit(ball, velocity);
        }
    }

	private void OnBottomAreaEntered(Node2D body)
	{
        Ball ball;
        if (!CheckIsValid(body, out ball))
            return;

        var velocity = ball.LinearVelocity;
		if (velocity.Y < 0)
		{
			velocity.Y *= -1;
			OnBrickHit(ball, velocity);
		}
    }

	private void OnLeftAreaEntered(Node2D body)
	{
        Ball ball;
        if (!CheckIsValid(body, out ball))
            return;

        var velocity = ball.LinearVelocity;
        if (velocity.X > 0)
        {
            velocity.X *= -1;
            OnBrickHit(ball, velocity);
        }
    }

    private void OnRightAreaEntered(Node2D body)
    {
        Ball ball;
        if (!CheckIsValid(body, out ball))
            return;

        var velocity = ball.LinearVelocity;
        if (velocity.X < 0)
        {
            velocity.X *= -1;
            OnBrickHit(ball, velocity);
        }
    }

    private void OnBrickHit(Ball ball, Vector2 velocity)
	{
		ball.Bounce(IsBrickHeavy);
		ball.LinearVelocity = velocity.Normalized() * (IsAccelerator ? 2 : 1) * ball.Speed;

		HP--;
		if (HP <= 0 && BrickType != BrickType.Unbreakable)
		{
			EmitSignal(SignalName.OnBrickDestroyed, Points*ball.Bonus);
            this.QueueFree();
		}

        if (IsDivider && !_hasDuplicated)
        {
            _hasDuplicated = true;
            ball.RaiseDuplicate();
        }
    }
}
