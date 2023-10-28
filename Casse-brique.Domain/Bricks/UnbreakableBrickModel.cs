using Casse_brique.Domain.Bricks;
using Godot;

namespace Cassebrique.Domain.Bricks
{
    public class UnbreakableBrickModel : BrickModel
    {
        public UnbreakableBrickModel(Vector2 position)
            : base(position, BrickType.Unbreakable, -1, 30, 0)
        {

        }
    }
}
