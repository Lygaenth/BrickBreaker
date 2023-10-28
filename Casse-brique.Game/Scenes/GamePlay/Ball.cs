using Casse_brique.Domain;
using Casse_brique.Domain.Enums;
using Godot;

/// <summary>
/// Ball class
/// </summary>
public partial class Ball : RigidBody2D
{

    #region subNodes
    private CollisionShape2D _collisionShape;
    private AnimatedSprite2D _sprite;
    private BounceSoundPlayer _bounceSoundPlayer;
	#endregion

    private bool _isAccelerated = false;
	private bool _launching;
    private Vector2 _screenSize;
	private int _rotationSpeed = 40;

	private BallModel _ballModel;

    public int Bonus { get => _ballModel.Bonus; } 

    public bool IsAttached { get => _ballModel.IsAttached; }

    public int ID { get => _ballModel.ID; }

    public void Setup(BallModel ballModel)
    {
        _ballModel = ballModel;
        _ballModel.Impulsed += OnBallImpulsed;
    }

    private void OnBallImpulsed(object sender, System.EventArgs e)
    {
		UpdateVelocity(_ballModel.LinearVelocity);
    }

    public void Destroy()
    {
        _ballModel.Destroy();
        _ballModel.Impulsed -= OnBallImpulsed;
        QueueFree();
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		_sprite = GetNode<AnimatedSprite2D>("Ballsprite");
		_bounceSoundPlayer = GetNode<BounceSoundPlayer>("BounceSoundPlayer");
        _collisionShape = GetNode<CollisionShape2D>("BallCollision");

        _screenSize = GetViewportRect().Size;
    }

    /// <summary>
    /// Force management
    /// </summary>
    /// <param name="state"></param>
    public override void _IntegrateForces(PhysicsDirectBodyState2D state)
    {
		if (_ballModel.IsAttached)
		{
			var velocity = new Vector2();
			if (Input.IsActionPressed("MoveLeft"))
				velocity.X -= 1;

			if (Input.IsActionPressed("MoveRight"))
				velocity.X += 1;

            if (Input.IsActionJustPressed("Action"))
            {
                _ballModel.Launch(velocity += Vector2.Up);
                return;
            }
			_ballModel.Move(velocity);
        }
    }

    /// <summary>
    /// Update ball velocity
    /// </summary>
    /// <param name="vector"></param>
    private void UpdateVelocity(Vector2 vector)
    {
		LinearVelocity = vector;
        AngularVelocity = Mathf.Sign(LinearVelocity.X) * 5;
    }

    /// <summary>
    /// Bounce on a surface and update internal stats
    /// </summary>
    /// <param name="isHeavy"></param>
    public void Bounce(bool isHeavy, int bonusModifier, AxisBounce axisBounce, Vector2 offset, SpecialEffect specialEffect)
	{
		_ballModel.Bounce(axisBounce, bonusModifier, offset);
        _bounceSoundPlayer.Play(isHeavy);

        if (specialEffect == SpecialEffect.Duplicate)
            Duplicate();
    }

    /// <summary>
    /// Hit lose zone
    /// </summary>
    public void HitLoseZone()
    {
        _ballModel.Destroy();
    }

    /// <summary>
    /// Request ball duplication
    /// </summary>
    public void Duplicate()
    {
        _ballModel.Duplicate(Position);
    }
}
