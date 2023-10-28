using Casse_brique.Domain.Bricks;
using Godot;

namespace Cassebrique.Domain.Bricks
{
    public class DividerBrickModel : BrickModel
    {
        public DividerBrickModel(Vector2 position)
            : base(position, BrickType.Divider, 1, 30, 20)
        {

        }
    }
}
