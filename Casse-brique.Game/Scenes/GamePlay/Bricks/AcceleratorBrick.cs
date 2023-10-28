using Casse_brique.Domain.Enums;
using Godot;

public partial class AcceleratorBrick : Brick
{
    protected override void OnBrickHit(Ball ball, AxisBounce axisBounce)
    {
        ball.Bounce(IsBrickHeavy, 5, axisBounce, Vector2.Zero);

		_brickModel.Hit(ball.Bonus);
    }
}
