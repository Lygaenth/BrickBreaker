using Cassebrique;
using Godot;

public partial class DividerBrick : Brick
{
    private bool _hasDuplicated;

    protected override void OnBrickHit(Ball ball, AxisBounce axisBounce)
    {
        ball.Bounce(IsBrickHeavy, 1, axisBounce);

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
