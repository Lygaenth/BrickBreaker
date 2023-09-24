using Godot;
using System;
using Cassebrique.Domain.Bricks;

public partial class SturdyBrick : Brick
{
    private CompressedTexture2D _4HPImg;
    private CompressedTexture2D _3HPImg;
    private CompressedTexture2D _2HPImg;
    private CompressedTexture2D _1HPImg;
    private Sprite2D _sprite;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        _4HPImg = GD.Load<CompressedTexture2D>("res://Images/Brick/brickTrulySturdy.png");
        _3HPImg = GD.Load<CompressedTexture2D>("res://Images/Brick/brick3Hp.png");
        _2HPImg = GD.Load<CompressedTexture2D>("res://Images/Brick/brickSturdy.png");
        _1HPImg = GD.Load<CompressedTexture2D>("res://Images/Brick/brickNormal.png");
        _sprite = GetNode<Sprite2D>("Border/Sprite2D");
    }

    protected override void OnBrickHit(Ball ball, Vector2 velocity)
    {
        ball.Bounce(IsBrickHeavy, -1);
        ApplyBounceVelocity(ball, velocity, ball.Speed);
        HP--;

        switch (HP)
        {
            case 3:
                _sprite.Texture = _3HPImg;
                break;
            case 2:
                _sprite.Texture = _2HPImg;
                break;
            case 1:
                _sprite.Texture = _1HPImg;
                break;
            default:
                RaiseBroken(ball);
                break;
        }
    }
}
