using Godot;

public partial class Boss : Node2D
{
	[Export]
	public int HP { get; set; }

	public int Speed { get; set; }

	[Signal]
	public delegate void BossDestroyedEventHandler();

	[Signal]
	public delegate void BossHitEventHandler(int hp);

	private AnimatedSprite2D _animatedSprite;
	private bool _phase2;
	private bool _isHit;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_animatedSprite = GetNode<AnimatedSprite2D>("Sprite");
		_animatedSprite.Animation = "Still";
		_animatedSprite.Play();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	public void OnBossHit(Node2D body)
	{
		if (body is not Ball ball)
			return;

		HP-= ball.Bonus > 2 ? 2 : 1;

        if (HP <= 5 && !_phase2)
        {
            Speed *= 2;
            _phase2 = true;
        }

        if (!_isHit)
		{
			_isHit = true;
			_animatedSprite.Animation = _phase2 ? "HitAngry" : "Hit";
			_animatedSprite.AnimationFinished += OnHitAnimationFinished;
			_animatedSprite.Play();
		}

		EmitSignal(Boss.SignalName.BossHit, HP);

		var repulsif = (ball.GlobalPosition - GlobalPosition);
		ball.LinearVelocity = (repulsif - ball.LinearVelocity).Normalized() * ball.Speed;

        ball.Bounce(true, 0);

		if (HP <= 0)
			EmitSignal(Boss.SignalName.BossDestroyed);
	}

    private void OnHitAnimationFinished()
    {
        _animatedSprite.AnimationFinished -= OnHitAnimationFinished;
        _animatedSprite.Animation = _phase2 ? "StillAngry" : "Still";
		_isHit = false;
    }
}
