using Godot;

public partial class Boss : Node2D
{
	[Export]
	public int HP { get; set; }

	public int Speed { get; set; }

	[Export]
	public Vector2 ProjectileSpawnPoint { get; set; }

	[Signal]
	public delegate void BossDestroyedEventHandler();

	[Signal]
	public delegate void BossHitEventHandler(int hp);

	[Signal]
	public delegate void BossSpawnProjectileEventHandler();

	private AnimatedSprite2D _animatedSprite;
	private Timer _attackTimer;

	private bool _phase2;
	private bool _isHit;
	private bool _isAttacking;
	private bool _requestAttackAfterRecover;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_animatedSprite = GetNode<AnimatedSprite2D>("Sprite");
		_animatedSprite.Animation = "Still";
		_animatedSprite.Play();

		_attackTimer = GetNode<Timer>("AttackTimer");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	public void OnBossHit(Node2D body)
	{
		if (body is not Ball ball)
			return;

		HP-= ball.Bonus > 3 ? 2 : 1;

        if (HP <= 5 && !_phase2)
        {
            Speed *= 2;
            _phase2 = true;
			_attackTimer.WaitTime = 1;
        }

        if (!_isHit)
		{
			_isHit = true;
			UpdateAnimationToHit();			
		}

		EmitSignal(Boss.SignalName.BossHit, HP);

		var reaction = (ball.GlobalPosition - GlobalPosition);
		ball.UpdateVelocity((reaction - ball.LinearVelocity).Normalized() * ball.Speed);
        ball.Bounce(true, 0);

		if (HP <= 0)
			EmitSignal(Boss.SignalName.BossDestroyed);
	}

	void UpdateAnimationToHit()
	{
        _animatedSprite.Animation = _phase2 ? "HitAngry" : "Hit";
		if (_isAttacking)
		{
			_animatedSprite.AnimationFinished -= OnAttackAnimationFinished;
			_animatedSprite.Stop();
			_isAttacking = false;
		}

        _animatedSprite.AnimationFinished += OnHitAnimationFinished;
        _animatedSprite.Play();
    }

    private void OnHitAnimationFinished()
    {
        _animatedSprite.AnimationFinished -= OnHitAnimationFinished;
		_isHit = false;
		if (_requestAttackAfterRecover)
		{
			_attackTimer.Start();
			UpdateAnimationToAttack();
			_requestAttackAfterRecover = false;
		}
		else
			UpdateAnimationToStill();
    }

    private void OnAttackTimeout()
	{
		if (_isHit)
		{
			_requestAttackAfterRecover = true;
			_attackTimer.Stop();
			return;
		}
		UpdateAnimationToAttack();
	}

	private void UpdateAnimationToAttack()
	{
        _isAttacking = true;

        _animatedSprite.AnimationFinished += OnAttackAnimationFinished;
        _animatedSprite.Animation = _phase2 ? "AttackingAngry" : "Attacking";
        _animatedSprite.Play();
    }

    private void OnAttackAnimationFinished()
    {
        _animatedSprite.AnimationFinished -= OnAttackAnimationFinished;
        UpdateAnimationToStill();		
		EmitSignal(Boss.SignalName.BossSpawnProjectile);
		_isAttacking = false;
    }

	private void UpdateAnimationToStill()
	{
        _animatedSprite.Animation = _phase2 ? "StillAngry" : "Still";
    }
}
