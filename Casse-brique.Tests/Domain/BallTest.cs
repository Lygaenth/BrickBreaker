using Casse_brique.Domain;
using Casse_brique.Domain.Enums;
using Casse_brique.Domain.Level;
using Godot;
using System.Drawing;

namespace Casse_brique.Tests.Domain
{
    public class BallTest
    {
        private bool _ballImpulsed;
        private List<int> _destroyedBalls;
        private BallCreationInfo _ballCreationInfo;

        [Test]
        public void TestBallLaunch()
        {
            _ballImpulsed = false;
            var ball = new BallModel(1);
            var launchVector = new Vector2(100, 400).Normalized();

            ball.Impulsed += OnBallImpulsed;
            ball.Launch(launchVector);

            Assert.That(ball.LinearVelocity, Is.EqualTo(launchVector * 500));
            Assert.IsTrue(_ballImpulsed);
            Assert.IsFalse(ball.IsAttached);
        }

        [Test]
        public void TestBallMove()
        {
            _ballImpulsed = false;
            var ball = new BallModel(1);
            var launchVector = new Vector2(100, 400).Normalized();

            ball.Impulsed += OnBallImpulsed;
            ball.Move(launchVector);

            Assert.That(ball.LinearVelocity, Is.EqualTo(launchVector * 500));
            Assert.IsTrue(_ballImpulsed);
            Assert.IsTrue(ball.IsAttached);
        }

        [Test]
        public void TestBallHorizontalBounce()
        {
            _ballImpulsed = false;
            var ball = new BallModel(1);
            ball.Launch(new Vector2(300, 500));

            ball.Impulsed += OnBallImpulsed;

            ball.Bounce(AxisBounce.X, 0, Vector2.Zero);

            ball.Impulsed -= OnBallImpulsed;

            Assert.That(ball.LinearVelocity.X, Is.LessThan(0));
            Assert.That(ball.LinearVelocity.Y, Is.GreaterThan(0));
            Assert.IsTrue(_ballImpulsed);
        }

        [Test]
        public void TestBallVerticalBounce()
        {
            _ballImpulsed = false;
            var ball = new BallModel(1);
            ball.Launch(new Vector2(300, 500));

            ball.Impulsed += OnBallImpulsed;

            ball.Bounce(AxisBounce.Y, 0, Vector2.Zero);

            ball.Impulsed -= OnBallImpulsed;

            Assert.That(ball.LinearVelocity.X, Is.GreaterThan(0));
            Assert.That(ball.LinearVelocity.Y, Is.LessThan(0));
            Assert.IsTrue(_ballImpulsed);
        }

        [Test]
        public void TestBonusModifierSpeedsUpBall()
        {
            var ball = new BallModel(1);
            ball.Launch(new Vector2(0, 500));
            int bonusModifier = 3;
            ball.Bounce(AxisBounce.Y, bonusModifier, Vector2.Zero);

            Assert.That(ball.LinearVelocity.X, Is.EqualTo(0));
            Assert.That(ball.LinearVelocity.Y, Is.EqualTo(-650));

            bonusModifier = 5;
            ball.Bounce(AxisBounce.Y, bonusModifier, Vector2.Zero);

            Assert.That(ball.LinearVelocity.X, Is.EqualTo(0));
            Assert.That(ball.LinearVelocity.Y, Is.EqualTo(750));
        }

        [Test]
        public void TestBounceWithMinimalAngle()
        {
            var ball = new BallModel(1);
            ball.Launch(new Vector2(500, 0));
            ball.Bounce(AxisBounce.X, 0, Vector2.Zero);

            Assert.That(ball.LinearVelocity.Y, Is.GreaterThan(100));

            ball.Launch(new Vector2(500, -30));
            ball.Bounce(AxisBounce.X, 0, Vector2.Zero);
            Assert.That(ball.LinearVelocity.Y, Is.LessThan(-100));
        }

        [Test]
        public void TestBallHitLoseZone()
        {
            _destroyedBalls = new List<int>();

            var ball = new BallModel(3);
            ball.Destroyed += OnBallDestroyed;
            ball.Destroy();

            Assert.That(_destroyedBalls[0], Is.EqualTo(3));
        }

        [Test]
        public void TestBallDuplicate()
        {
            _ballCreationInfo = null;
            var position = new Vector2(500, 0);
            var ball = new BallModel(2);
            ball.Duplicated += OnBallDuplicated;
            ball.Duplicate(position);
            ball.Duplicated -= OnBallDuplicated;
            Assert.IsNotNull(_ballCreationInfo);
            Assert.That(_ballCreationInfo.ID, Is.EqualTo(ball.ID));
            Assert.That(_ballCreationInfo.Position, Is.EqualTo(position));
            Assert.That(_ballCreationInfo.InitialVelocity.X, Is.EqualTo(0));
            Assert.That(_ballCreationInfo.InitialVelocity.Y, Is.EqualTo(0));
        }

        private void OnBallDuplicated(object? sender, BallCreationInfo creationInfo)
        {
            _ballCreationInfo = creationInfo;   
        }

        private void OnBallImpulsed(object? sender, EventArgs e)
        {
            _ballImpulsed = true;
        }

        private void OnBallDestroyed(object? sender, int ballID)
        {
            _destroyedBalls.Add(ballID);
        }
    }
}
