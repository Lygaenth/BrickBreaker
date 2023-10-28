using Cassebrique.Domain.Bricks;
using Godot;

namespace Casse_brique.Domain.Bricks
{
    public class SturdyBrickModel : BrickModel
    {
        public SturdyBrickModel(Vector2 position)
            : base(position, BrickType.Sturdy, 2, 30, 20)
        {
        }
    }
}
