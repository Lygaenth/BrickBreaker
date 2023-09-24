using Godot;

public partial class DividerBrick : Brick
{
    private bool _hasDuplicated;

    protected override void OnBrickHit(Ball ball, Vector2 velocity)
    {
        ball.Bounce(IsBrickHeavy, 1);
        ApplyBounceVelocity(ball, velocity, ball.Speed);

        HP--;

        if (!_hasDuplicated)
        {
            _hasDuplicated = true;
            ball.RaiseDuplicate();
        }

        if (HP <= 0)
            RaiseBroken(ball);
    }
}
