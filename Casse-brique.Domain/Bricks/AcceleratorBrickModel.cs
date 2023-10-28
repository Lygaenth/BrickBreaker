using Cassebrique.Domain.Bricks;
using Godot;

namespace Casse_brique.Domain.Bricks
{
    public class AcceleratorBrickModel :BrickModel
    {
        public AcceleratorBrickModel(Vector2 position)
            : base(position, BrickType.Accelerator, 1, 30, 20)
        {

        }
    }
}
