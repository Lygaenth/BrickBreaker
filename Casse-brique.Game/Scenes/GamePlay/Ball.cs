using Cassebrique.Helper;
using Godot;

/// <summary>
/// Ball class
/// </summary>
public partial class Ball : RigidBody2D
{

	private const string MovingAnim = "Moving";
    private const string StillAnim = "Still";
    private const float PitchRatio = 1.0595f;
	private float[] _highPitch = new float[] { 1, 1 / Mathf.Pow(PitchRatio, 4f), 1 / Mathf.Pow(PitchRatio, 7f), 1/ Mathf.Sqrt2 };// 1 / Mathf.Pow(PitchRatio, 2f), 1 / Mathf.Pow(PitchRatio, 4f), 1 / Mathf.Pow(PitchRatio, 6f), 1 / Mathf.Pow(PitchRatio, 7f), 1 / Mathf.Pow(PitchRatio, 8f), 1 / Mathf.Pow(PitchRatio, 10f), 1 / Mathf.Pow(PitchRatio, 12f) };
    private float[] _lowPitch = new float[] { 0.5f, 0.5f / Mathf.Pow(PitchRatio, 4f), 0.5f / Mathf.Pow(PitchRatio, 7f), 0.5f / Mathf.Sqrt2 };
	private int _lowPitchIndex = 0;
    private int _highPitchIndex = 0;
    #region subNodes
    private CollisionShape2D _collisionShape;
    private AnimatedSprite2D _sprite;
    private AudioStreamPlayer _bounceSoundPlayer;
	#endregion

	private float _scale = 0.3f;
    private bool _isAccelerated = false;
	private int _bonus = 0;
	private bool _launching;
    private Vector2 _screenSize;

	public float Bonus { get => (100f + _bonus * 10) / 100; }

	public int ID { get; set; }

    [Export]
	public bool IsAttachedToBar { get; set; }

	[Export]
	public int Speed { get; set; }

	[Export]
	public int DetachedSpeedBall { get; set; }

    [Export]
    public bool CanMove { get; set; }

    [Signal]
    public delegate void OnDuplicateBallEventHandler(Ball ball);

	[Signal]
	public delegate void OnHitEventHandler(int ID, int intensity);

	private Vector2 _impulse = Vector2.Zero;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_sprite = GetNode<AnimatedSprite2D>("Ballsprite");
		_bounceSoundPlayer = GetNode<AudioStreamPlayer>("BounceSoundPlayer");
        _collisionShape = GetNode<CollisionShape2D>("BallCollision");

        _screenSize = GetViewportRect().Size;

		UpdateScale();
		if (IsAttachedToBar)
			UpdateAnimation(StillAnim);
		else
			UpdateAnimation(MovingAnim);

        Speed = GameConstants.BaseSpeed;
    }

	private void UpdateScale()
	{
		if (_sprite != null)
	        _sprite.Scale = Vector2.One * _scale;
		if (_collisionShape != null)
	        _collisionShape.Scale = Vector2.One * _scale;
    }

	/// <summary>
	/// Force management
	/// </summary>
	/// <param name="state"></param>
    public override void _IntegrateForces(PhysicsDirectBodyState2D state)
    {
		if (IsAttachedToBar)
		{
			var velocity = new Vector2();
			if (Input.IsActionPressed("MoveLeft"))
				velocity.X -= 1;

			if (Input.IsActionPressed("MoveRight"))
				velocity.X += 1;

            if (Input.IsActionJustPressed("Action"))
            {
                IsAttachedToBar = false;
                ApplyImpulse(Vector2.Up * Speed);
                return;
            }

            LinearVelocity = velocity * Speed;
		}
    }

    /// <summary>
    /// Accelerate the ball to max speed and max bonus
    /// </summary>
    public void Accelerate()
	{
		Speed = (int)(GameConstants.BaseSpeed * 1.5);
		if (_bonus < 5)
			_bonus = 5;
	}

	/// <summary>
	/// Bounce on a surface and update internal stats
	/// </summary>
	/// <param name="isHeavy"></param>
	public void Bounce(bool isHeavy, int bonusModifier)
	{
		_bounceSoundPlayer.PitchScale = isHeavy ? _lowPitch[_lowPitchIndex] : _highPitch[_highPitchIndex];
        _bounceSoundPlayer.Play();

		if (isHeavy)
			_lowPitchIndex = (_lowPitchIndex+1) % 4;
		else
            _highPitchIndex = (_highPitchIndex + 1) % 4;

        if (bonusModifier < 0 && _bonus + bonusModifier < 0)
			_bonus = 0;
		else if (bonusModifier > 0 && _bonus + bonusModifier > 5)
			_bonus = 5;
		else
			_bonus += bonusModifier;

        Speed = GameConstants.BaseSpeed * (100 + _bonus * 10) / 100;

        if (LinearVelocity.X >= 0)
			_sprite.Play();
		else
			_sprite.PlayBackwards(MovingAnim);

		EmitSignal(SignalName.OnHit, ID, _bonus);
    }

	/// <summary>
	/// Update current animation
	/// </summary>
	/// <param name="animationName"></param>
	private void UpdateAnimation(string animationName)
	{
		_sprite.Animation = animationName;
		_sprite.Play();
    }

	/// <summary>
	/// Raise an event to duplicate this ball
	/// </summary>
	public void RaiseDuplicate()
	{
		EmitSignal(SignalName.OnDuplicateBall, this);
	}
}
