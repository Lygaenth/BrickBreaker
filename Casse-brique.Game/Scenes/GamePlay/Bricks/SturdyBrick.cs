using Casse_brique.Domain.Enums;
using Godot;

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
        ConvertHPToTexture();
    }

    protected override void OnBrickHit(Ball ball, AxisBounce axisBounce)
    {
        base.OnBrickHit(ball, axisBounce);
        ConvertHPToTexture();
    }

    private void ConvertHPToTexture()
    {
        switch (HP)
        {
            case 4:
                _sprite.Texture = _4HPImg;
                break;
            case 3:
                _sprite.Texture = _3HPImg;
                break;
            case 2:
                _sprite.Texture = _2HPImg;
                break;
            case 1:
                _sprite.Texture = _1HPImg;
                break;
        }
    }
}
