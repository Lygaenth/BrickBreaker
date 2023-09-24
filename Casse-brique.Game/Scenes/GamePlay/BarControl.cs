using Godot;

public partial class BarControl : Area2D
{
	private Timer _bashEffectTimer;
	private Timer _bashCdTimer;
	private AudioStreamPlayer _bashSound;
	private Sprite2D _bashSprite;

	[Export]
	public int Speed { get; set; }

    public bool CanMove { get; set; }
	private bool _isBashing;
	private bool _isBashInCoolDown;

    private Vector2 _screenSize;
	private Vector2 _velocity;

	private Vector2 _startPosition;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_screenSize = GetViewportRect().Size;

		_bashEffectTimer = GetNode<Timer>("BashTimer");
        _bashEffectTimer.Timeout += OnBashTimeout;
		_bashCdTimer = GetNode<Timer>("BashCoolDownTimer");
		_bashSprite = GetNode<Sprite2D>("AccentuationSprite");
        _bashCdTimer.Timeout += OnBashCdTimeout;
		_bashSound = GetNode<AudioStreamPlayer>("BashSound");
		_startPosition = Position;
		_velocity = Vector2.Zero;
    }

    private void OnBashCdTimeout()
    {
		_isBashInCoolDown = false;
		_bashSprite.Show();
    }

    private void OnBashTimeout()
    {
		_isBashing = false;
    }

    // Move bar on inputs
    public override void _Process(double delta)
	{
		if (!CanMove)
			return;

		var velocity = Vector2.Zero;

		if (Input.IsActionPressed("MoveLeft"))
			velocity.X -= 1;

		if (Input.IsActionPressed("MoveRight"))
			velocity.X += 1;

		if (Input.IsActionJustPressed("Action") && !_isBashInCoolDown)
		{
			velocity.Y += -1;
			_isBashing = true;
			_isBashInCoolDown = true;
			_bashSound.Play();
			_bashEffectTimer.Start();
			_bashCdTimer.Start();
			_bashSprite.Hide();
		}

		if (!_isBashing && _isBashInCoolDown && Position.Y < _startPosition.Y)
		{
            velocity.Y += 0.5f;
        }

        _velocity = velocity;

		var position = Position + velocity * Speed * (float)delta;
		position.X = Mathf.Clamp(position.X, 50, _screenSize.X - 50);

        Position = position;
	}

	/// <summary>
	/// On ball collision
	/// </summary>
	/// <param name="body"></param>
    private void OnAreaHit(Node2D body)
    {
        if (body is not Ball ball || ball.IsAttachedToBar)
            return;

        var velocity = ball.LinearVelocity;
        velocity.Y = -1 * ball.Speed;
		velocity.X += _velocity.X * ball.Speed;
        ball.Bounce(true, _isBashing ? 5 : 0);
        ball.LinearVelocity = velocity.Normalized() * ball.Speed;
    }
}
