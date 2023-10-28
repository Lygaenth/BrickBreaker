using Casse_brique.Domain.Bricks;
using Godot;

namespace Cassebrique.Domain.Bricks
{ 
    public class TrulySturdyBrickModel : BrickModel
    {
        public TrulySturdyBrickModel(Vector2 position)
            : base(position, BrickType.TrulySturdy, 4, 30, 40)
        {

        }
    }
}
