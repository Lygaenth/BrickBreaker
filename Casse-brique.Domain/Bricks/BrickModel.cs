using Casse_brique.Domain.Enums;
using Cassebrique.Domain.Bricks;
using Godot;
using System.Xml.Serialization;

namespace Casse_brique.Domain.Bricks
{
    public class BrickModel
    {
        private int _hp;
        public int HP { get => _hp; }
        public Vector2 Position { get; }

        private readonly int _points;

        public BrickType BrickType { get; }
        public event EventHandler<int> Broken;

        private int _invulnerabilityDuration;
        private DateTime _lastHitTime;

        public virtual bool IsHeavy { get => HP > 1; }

        public BrickModel(Vector2 position, BrickType brickType = BrickType.Normal, int maxHP = 1, int invulnerabilityDuration = 30, int points = 10)
        {
            Position = position;
            BrickType = brickType;
            _hp = maxHP;
            _invulnerabilityDuration = invulnerabilityDuration;
            _points = points;
        }

        public void Hit(float multiplier)
        {
            if ((DateTime.Now - _lastHitTime).Milliseconds < _invulnerabilityDuration)
                return;

            _lastHitTime = DateTime.Now;
            _hp = Math.Max(0, _hp -1);

            if (HP == 0 && Broken != null)
                Broken(this, (int)(_points * multiplier));
        }

        public virtual SpecialEffect GetSpecialEffect()
        {
            return SpecialEffect.None;
        }
    }
}
