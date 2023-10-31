using Godot;
using System.Drawing;

namespace Casse_brique.Domain
{
    public class BossModel
    {
        private int _hp;
        public int HP { get => _hp; }

        private int _speed;
        public int Speed { get => _speed; }

        private int _attackRate;
        public int AttackRate { get => _attackRate; }

        private int _scoreValue = 1000;

        private bool _phase2;

        private List<List<Point>> _paths;
        private int _bossPathIndex;

        public event EventHandler<int> Destroyed;

        public event EventHandler<bool> HasBeenHit;

        public event EventHandler ChangedPhase;

        public string BossName { get; }

        public BossModel(string bossName, int maxHp, List<List<Point>> pathLists)
        {
            BossName = bossName;
            _hp = maxHp;
            _speed = 100;
            _attackRate = 2;
            _paths = pathLists;
        }

        public void Hit(int velocity)
        {
            _hp -= velocity > 3 ? 2 : 1;

            if (HP <= 5 && !_phase2)
            {
                _speed *= 2;
                _phase2 = true;
                _attackRate = 1;

                if (ChangedPhase != null)
                    ChangedPhase(this, new EventArgs());
            }

            if (HP <= 0 && Destroyed != null)
                Destroyed(this, _scoreValue);
        }

        public List<Point> GetNextBossPath()
        {
            var curve = new List<Point>();
            foreach (var point in _paths[_bossPathIndex])
                curve.Add(point);
            _bossPathIndex = (_bossPathIndex + 1) % _paths.Count;
            return curve;
        }
    }
}
