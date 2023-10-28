using Casse_brique.Domain.Enums;
using Cassebrique;
using Cassebrique.Scenes.GamePlay.BarControl;
using Godot;

public partial class BarControl : Area2D
{

	[Export]
	public int Speed { get; set; }

    public bool CanMove { get; set; }

    private Vector2 _screenSize;
	private Vector2 _velocity;
	private Vector2 _startPosition;

    private BashManager _bashManager;
	private AudioStreamPlayer _soundPlayer;
	private AnimatedSprite2D _animatedSprite;

	private bool _isRecovering;

    [Signal]
	public delegate void HitByProjectileEventHandler(int damage);

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_screenSize = GetViewportRect().Size;

		_bashManager = new BashManager(GetNode<Timer>("BashTimer"), 
			GetNode<Timer>("BashCoolDownTimer"),
            GetNode<AudioStreamPlayer>("BashSound"),
            GetNode<Sprite2D>("AccentuationSprite"));

		_animatedSprite = GetNode<AnimatedSprite2D>("Sprite2D");
		_soundPlayer = GetNode<AudioStreamPlayer>("BarSound");

		_animatedSprite.Animation = "Still";
		_startPosition = Position;
		_velocity = Vector2.Zero;
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

		if (Input.IsActionJustPressed("Action") && _bashManager.CanBash())
		{
			velocity.Y += -1;
			_bashManager.Bash();
		}

		if (_bashManager.IsRecovering() && Position.Y < _startPosition.Y)
            velocity.Y += 0.5f;

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
		if (body is Ball ball && !ball.IsAttached)
			ReactToBall(ball);

		if (body is Projectile projectile)
			ReactToProjectile(projectile);
    }

	/// <summary>
	/// React to ball collision
	/// </summary>
	/// <param name="ball"></param>
	private void ReactToBall(Ball ball)
	{
		ball.Bounce(true, _bashManager.IsBashing() ? 5 : 0, AxisBounce.Y, _velocity * Speed);
    }

	/// <summary>
	/// React to projectile collision 
	/// </summary>
	/// <param name="projectile"></param>
	private void ReactToProjectile(Projectile projectile)
	{
		if (_isRecovering)
			return;

		_isRecovering = true;
        EmitSignal(SignalName.HitByProjectile, -1);
        projectile.QueueFree();
        _soundPlayer.Play();
        
		_animatedSprite.AnimationFinished += OnHitAnimationFinished;
		_animatedSprite.Animation = "Hit";
		_animatedSprite.Play();	
    }

    private void OnHitAnimationFinished()
    {
		_isRecovering = false;
        _animatedSprite.AnimationFinished -= OnHitAnimationFinished;
        _animatedSprite.Animation = "Still";
    }
}
