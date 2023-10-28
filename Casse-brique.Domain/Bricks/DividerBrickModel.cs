using Casse_brique.Domain;
using Casse_brique.Domain.Bricks;
using Casse_brique.Domain.Enums;
using Godot;

namespace Cassebrique.Domain.Bricks
{
    public class DividerBrickModel : BrickModel
    {
        private bool _canDuplicate = true;

        public DividerBrickModel(Vector2 position)
            : base(position, BrickType.Divider, 1, 30, 20)
        {

        }

        public override SpecialEffect GetSpecialEffect()
        {
            return HP > 0 ? SpecialEffect.Duplicate : SpecialEffect.None;
        }

    }
}
