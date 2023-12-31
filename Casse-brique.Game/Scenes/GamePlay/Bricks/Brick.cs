using Cassebrique;
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
	public bool IsBrickHeavy { get; set; }

    [Signal]
	public delegate void OnBrickDestroyedEventHandler(int point);

	private DateTime _lastHitTime;
	
	public Brick()
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

		if ((hitTime - _lastHitTime).Milliseconds < 30)
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
            OnBrickHit(ball, AxisBounce.Y);
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
			OnBrickHit(ball, AxisBounce.Y);
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
            OnBrickHit(ball, AxisBounce.X);
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
            OnBrickHit(ball, AxisBounce.X);
        }
    }

    protected void RaiseBroken(Ball ball)
    {
        EmitSignal(SignalName.OnBrickDestroyed, Points * ball.Bonus);
        this.QueueFree();
    }

    protected void ApplyBounceVelocity(Ball ball, Vector2 velocity, int speed)
    {
        ball.LinearVelocity = velocity.Normalized() * speed;
    }

    protected virtual void OnBrickHit(Ball ball, AxisBounce axisBounce)
	{
		ball.Bounce(IsBrickHeavy, 1, axisBounce);

        HP--;

        if (HP <= 0)
            RaiseBroken(ball);
    }
}
