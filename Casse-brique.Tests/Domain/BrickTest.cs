using Casse_brique.Domain.Bricks;
using Cassebrique.Domain.Bricks;
using Godot;

namespace Casse_brique.Tests.Domain
{
    public class BrickTests
    {
        private bool _brickBroken = false;
        private int _score = 0;

        [Test]
        public void TestBrickLoseHPWhenHit()
        {
            var brick = new BrickModel(Vector2.Zero, BrickType.Normal, 2, 30, 0);

            brick.Hit(1);

            Assert.That(brick.HP, Is.EqualTo(1));
        }

        [Test]
        public void TestBrickMinimumHPIs0()
        {
            var brick = new BrickModel(Vector2.Zero, BrickType.Normal, 2, 0, 0);

            brick.Hit(1);
            brick.Hit(1);
            brick.Hit(1);
            brick.Hit(1);
            brick.Hit(1);

            Assert.That(brick.HP, Is.EqualTo(0));
        }

        [Test]
        public void TestBrickImmuneToDamageForATime()
        {
            var brick = new BrickModel(Vector2.Zero, BrickType.Normal, 5, 30, 0);
            brick.Hit(1);
            brick.Hit(1);
            Assert.That(brick.HP, Is.EqualTo(4));
            Thread.Sleep(30);

            brick.Hit(1);
            Assert.That(brick.HP, Is.EqualTo(3));
        }

        [Test]
        public void BrickRaiseEventOnBroke()
        {
            _brickBroken = false;

            var brick = new BrickModel(Vector2.Zero, BrickType.Normal, 1, 0, 0);
            brick.Broken += BrickHasBeenBroken;
            brick.Hit(1);
            brick.Broken -= BrickHasBeenBroken;

            Assert.IsTrue(_brickBroken);
            Assert.That(brick.HP, Is.EqualTo(0));
        }

        [Test]
        public void BrokenBrickSendsAssociatedPoints()
        {
            _brickBroken = false;
            _score = 0;

            var brick = new BrickModel(Vector2.Zero, BrickType.Normal, 1, 0, 30);

            brick.Broken += BrickHasBeenBroken;
            brick.Hit(1);

            Assert.That(_score, Is.EqualTo(30));
            brick.Broken -= BrickHasBeenBroken;

            brick = new BrickModel(Vector2.Zero, BrickType.Normal, 1, 0, 20);

            brick.Broken += BrickHasBeenBroken;
            brick.Hit(2);
            brick.Broken -= BrickHasBeenBroken;

            Assert.That(_score, Is.EqualTo(70));
        }

        private void BrickHasBeenBroken(object? sender, int points)
        {
            _brickBroken = true;
            _score += points;
        }
    }
}