using Casse_brique.Domain;
using Casse_brique.Domain.Enums;
using Godot;

public partial class Boss : Node2D
{
	[Export]
	public Vector2 ProjectileSpawnPoint { get; set; }

	[Signal]
	public delegate void BossSpawnProjectileEventHandler();

	public int Speed { get => _model.Speed; }

	public int HP { get => _model.HP; }

	private AnimatedSprite2D _animatedSprite;
	private Timer _attackTimer;

	private bool _phase2;
	private bool _isHit;
	private bool _isAttacking;
	private bool _requestAttackAfterRecover;
	private int _bossPathIndex = -1;

	private BossModel _model;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_animatedSprite = GetNode<AnimatedSprite2D>("Sprite");
		_animatedSprite.Animation = "Still";
		_animatedSprite.Play();

		_attackTimer = GetNode<Timer>("AttackTimer");
	}

	public void Initialize(BossModel model)
	{
		_model = model;
        _model.Destroyed += OnDestroyed;
	}

    private void OnDestroyed(object sender, int e)
    {
		QueueFree();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
		if(GetParent() is PathFollow2D pathFollow)
		{
            var progress = pathFollow.Progress + (float)delta * Speed;
            pathFollow.Progress = progress;
            if (pathFollow.Progress < progress)
                LoadNextBossPath(pathFollow);
        }
	}

    private void LoadNextBossPath(PathFollow2D pathFollow)
    {
        (pathFollow.GetParent() as Path2D).Curve = _model.GetNextBossPath();
        pathFollow.Progress = 0;
    }

	public void OnBossHit(Node2D body)
	{
		if (body is not Ball ball)
			return;

		_model.Hit(ball.Bonus);

        if (!_isHit)
		{
			_isHit = true;
			UpdateAnimationToHit();			
		}

        ball.Bounce(true, 0, AxisBounce.XY, Vector2.Zero);
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
