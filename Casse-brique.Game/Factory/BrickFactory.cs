using Casse_brique.Domain.API;
using Casse_brique.Domain.Bricks;
using Cassebrique.Domain.Bricks;
using Godot;

namespace Cassebrique.Factory
{
    /// <summary>
    /// Factory for bricks
    /// </summary>
    public class BrickFactory : IBrickFactory
    {
        private const string BrickPath = "res://Scenes/GamePlay/Bricks/Brick.tscn";
        private const string AcceleratorBrickPath = "res://Scenes/GamePlay/Bricks/AcceleratorBrick.tscn";
        private const string DividerBrickPath = "res://Scenes/GamePlay/Bricks/DividerBrick.tscn";
        private const string SturdyBrickPath = "res://Scenes/GamePlay/Bricks/SturdyBrick.tscn";
        private const string TrulySturdyBrickPath = "res://Scenes/GamePlay/Bricks/TrulySturdyBrick.tscn";
        private const string UnbreakableBrickPath = "res://Scenes/GamePlay/Bricks/UnbreakableBrick.tscn";
        private readonly PackedScene _brickPackedScene;
        private readonly PackedScene _acceleratorPackedScene;
        private readonly PackedScene _dividerBrickPackedScene;
        private readonly PackedScene _sturdyBrickPackedScene;
        private readonly PackedScene _trulySturdyBrickPackedScene;
        private readonly PackedScene _unbreakablePackedScene;

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
            _acceleratorPackedScene = ResourceLoader.Load<PackedScene>(AcceleratorBrickPath);
            _dividerBrickPackedScene = ResourceLoader.Load<PackedScene>(DividerBrickPath);
            _sturdyBrickPackedScene = ResourceLoader.Load<PackedScene>(SturdyBrickPath);
            _trulySturdyBrickPackedScene = ResourceLoader.Load<PackedScene>(TrulySturdyBrickPath);
            _unbreakablePackedScene = ResourceLoader.Load<PackedScene>(UnbreakableBrickPath);
        }

        /// <summary>
        /// Create brick
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Brick CreateBrick(BrickModel brickModel)
        {
            Brick brick;
            switch (brickModel.BrickType)
            {
                case BrickType.Sturdy:
                    brick = _sturdyBrickPackedScene.Instantiate<Brick>();
                    break;
                case BrickType.TrulySturdy:
                    brick = _trulySturdyBrickPackedScene.Instantiate<Brick>();
                    break;
                case BrickType.Divider:
                    brick = _dividerBrickPackedScene.Instantiate<Brick>();
                    break;
                case BrickType.Accelerator:
                    brick = _acceleratorPackedScene.Instantiate<Brick>();
                    break;
                case BrickType.Unbreakable:
                    brick = _unbreakablePackedScene.Instantiate<Brick>();
                    break;
                default:
                    brick = _brickPackedScene.Instantiate<Brick>();
                    break;
            }
            brick.Setup(brickModel);
            brick.Position = brickModel.Position;
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
