using Casse_brique.Domain;
using System.Drawing;

namespace Casse_brique.Tests.Domain
{
    public class BossTest
    {
        private bool _isBossDestroyed = false;

        [Test]
        public void TestBossLoseHpOnHit()
        {
            var boss = new BossModel("boss", 10, new List<List<Point>>());
            boss.Hit(1);
            Assert.That(boss.HP, Is.EqualTo(9));
        }

        [Test]
        public void TestBossDiesOnZeroHp()
        {
            _isBossDestroyed = false;
            var boss = new BossModel("boss", 1, new List<List<Point>>());
            boss.Destroyed += OnBossDestroyed;
            boss.Hit(0);
            Assert.That(boss.HP, Is.EqualTo(0));
            Assert.IsTrue(_isBossDestroyed);
        }

        [Test]
        public void TestGetNextPath()
        {
            var paths = new List<List<Point>>();
            var path1 = new List<Point> { new Point(0,0), new Point(1,0), new Point(1,1), new Point(0,1) };
            var path2 = new List<Point> { new Point(1, 2), new Point(2, 2), new Point(2, 1) };
            paths.Add(path1);
            paths.Add(path2);
            var boss = new BossModel("boss", 1, paths);
            var curve1 = boss.GetNextBossPath();
            Assert.That(curve1.Count, Is.EqualTo(4));

            var curve2 = boss.GetNextBossPath();
            Assert.That(curve2.Count, Is.EqualTo(3));

            var curve3 = boss.GetNextBossPath();
            Assert.That(curve1, Is.EqualTo(curve3));
        }

        private void OnBossDestroyed(object? sender, int e)
        {
            _isBossDestroyed = true;
        }
    }
}
