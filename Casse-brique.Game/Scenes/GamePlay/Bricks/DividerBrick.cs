using Casse_brique.Domain.Enums;
using Godot;

public partial class DividerBrick : Brick
{
    private bool _hasDuplicated;

    protected override void OnBrickHit(Ball ball, AxisBounce axisBounce)
    {
        ball.Bounce(IsBrickHeavy, 1, axisBounce, Vector2.Zero);
        _brickModel.Hit(ball.Bonus);

        if (!_hasDuplicated)
        {
            _hasDuplicated = true;
            ball.Duplicate();
        }
    }
}
