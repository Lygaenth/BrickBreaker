using Casse_brique.Domain.Bricks;
using Casse_brique.Domain.Enums;
using Cassebrique;
using Cassebrique.Domain.Bricks;
using Godot;
using System;

public partial class Brick : Node2D
{
	public int HP { get => _brickModel.HP; }

	public BrickType BrickType { get => _brickModel.BrickType; }

    public int Points { get => _brickModel.HP; }

	public bool IsBrickHeavy { get => _brickModel.IsHeavy; }

    protected BrickModel _brickModel;

    [Signal]
	public delegate void OnBrickDestroyedEventHandler();

	private DateTime _lastHitTime;
	
	public Brick()
	{
		_lastHitTime = DateTime.Now;
	}

    public void Initialize(BrickModel brickModel)
    {
        _brickModel = brickModel;
        _brickModel.Broken += OnBroken;
    }

    private void OnBroken(object sender, int e)
    {
        EmitSignal(SignalName.OnBrickDestroyed);
        _brickModel.Broken -= OnBroken;
        this.QueueFree();
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

    protected virtual void OnBrickHit(Ball ball, AxisBounce axisBounce)
	{
		ball.Bounce(IsBrickHeavy, 1, axisBounce, Vector2.Zero);
        _brickModel.Hit(ball.Bonus);
    }
}
