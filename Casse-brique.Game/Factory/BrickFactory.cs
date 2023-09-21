using Cassebrique.Domain.Bricks;
using Godot;

namespace Cassebrique.Factory
{
    /// <summary>
    /// Factory for bricks
    /// </summary>
    public class BrickFactory : IBrickFactory
    {
        private const string BrickPath = "res://Scenes/GamePlay/Brick.tscn";
        private readonly PackedScene _brickPackedScene;

        #region Colors 
        private readonly Color NormalColor = Color.Color8(255, 0, 0, 255);
        private readonly Color SturdyColor = Color.Color8(0, 255, 0, 255);
        private readonly Color DividerColor = Color.Color8(220, 110, 0, 255);
        private readonly Color AcceleratorColor = Color.Color8(175, 175, 0, 255);
        private readonly Color TrulySturdyColor = Color.Color8(0, 0, 255, 255);
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public BrickFactory()
        {
            _brickPackedScene = ResourceLoader.Load<PackedScene>(BrickPath);
        }

        /// <summary>
        /// Create brick
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Brick CreateBrick(BrickDto model)
        {
            var brick = _brickPackedScene.Instantiate() as Brick;
            brick.Position = new Vector2(model.X, model.Y);
            brick.BrickType = model.BrickType;
            switch (model.BrickType)
            {
                case BrickType.Normal:
                    SetColor(brick, NormalColor);
                    break;
                case BrickType.Sturdy:
                    brick.HP = 2;
                    SetColor(brick, SturdyColor);
                    break;
                case BrickType.TrulySturdy:
                    brick.HP = 3;
                    SetColor(brick, TrulySturdyColor);
                    break;
                case BrickType.Divider:
                    brick.IsDivider = true;
                    SetColor(brick, DividerColor);
                    break;
                case BrickType.Accelerator:
                    brick.IsAccelerator = true;
                    SetColor(brick, AcceleratorColor);
                    break;
                case BrickType.Unbreakable:
                    brick.HP = -1;
                    break;
            }
            return brick;
        }

        /// <summary>
        /// Set color of the brick
        /// </summary>
        /// <param name="brick"></param>
        /// <param name="color"></param>
        private void SetColor(Brick brick, Color color)
        {
            var sprite = brick.GetNode<Sprite2D>("Sprite2D");
            sprite.Texture = new GradientTexture2D();
            var texture = sprite.Texture as GradientTexture2D;
            texture.Gradient = new Gradient();
            // remove default points
            while (texture.Gradient.GetPointCount() > 1)
                texture.Gradient.RemovePoint(1);
            texture.Width = 100;
            texture.Height = 20;
            texture.Gradient.AddPoint(0, color);
            texture.Gradient.AddPoint(1, color.Darkened(0.7f));
        }
    }
}
