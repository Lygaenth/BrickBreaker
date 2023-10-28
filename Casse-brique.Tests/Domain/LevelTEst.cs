using Casse_brique.Domain.API;
using Casse_brique.Domain.Level;
using Cassebrique.Domain.API;
using Cassebrique.Domain.Bricks;
using Moq;
using NUnit.Framework.Constraints;
using System.Drawing;

namespace Casse_brique.Tests.Domain
{
    public class LevelTest
    {
        private Mock<ILevelService> _levelService;

        private GameState _gameState;
        private BallCreationInfo _ballCreationInfo;
        private int _numberOfBallCreated;
        private int _finalScore;
        private bool _resetBarPositionRequested;
        private bool _levelEnded;

        [SetUp]
        public void Setup()
        {
            _levelService = new Mock<ILevelService>();
        }

        [Test]
        public void TestLoadingLevelWithoutBoss()
        {
            var level = new LevelModel(_levelService.Object);

            var levelDto = new LevelDto();
            levelDto.Bricks = new List<BrickDto>() { new BrickDto() { BrickType = BrickType.Divider, Id = 1, X = 10, Y = 20 } };
            levelDto.BossName = "";
            levelDto.BossUri = "";
            levelDto.BossPaths = new List<List<Point>>();
            levelDto.HasBoss = false;
            levelDto.ID = 1;

            _levelService.Setup(s => s.GetLevel(1)).Returns(levelDto);

            level.LoadLevel();

            Assert.That(level.Bricks.Count, Is.EqualTo(1));
            var brick = level.Bricks[0];
            Assert.That(brick.BrickType, Is.EqualTo(BrickType.Divider));
            Assert.That(brick.Position.X, Is.EqualTo(10));
            Assert.That(brick.Position.Y, Is.EqualTo(20));
            Assert.That(level.Lives, Is.EqualTo(3));
            Assert.That(level.CurrentStage, Is.EqualTo("1"));
            Assert.That(level.Balls.Count, Is.EqualTo(1));
            Assert.IsNull(level.Boss);
        }

        [Test]
        public void TestScoreUpdate()
        {
            _gameState = null;
            var level = new LevelModel(_levelService.Object);
            level.GameStateUpdated += OnGameStateUpdated;
            level.AddScore(100);
            level.GameStateUpdated -= OnGameStateUpdated;
            Assert.That(level.Score, Is.EqualTo(100));
            Assert.NotNull(_gameState);
            Assert.That(_gameState.Score, Is.EqualTo(100));
        }

        [Test]
        public void TestLivesUpdate()
        {
            _gameState = null;
            var level = new LevelModel(_levelService.Object);
            level.GameStateUpdated += OnGameStateUpdated;
            level.Damage(1);
            level.GameStateUpdated -= OnGameStateUpdated;
            Assert.That(level.Lives, Is.EqualTo(2));
            Assert.NotNull(_gameState);
            Assert.That(_gameState.Lives, Is.EqualTo(2));
        }

        [Test]
        public void TestBrickBroken()
        {
            _gameState = null;

            var levelDto = new LevelDto();
            levelDto.Bricks = new List<BrickDto>() { new BrickDto() { BrickType = BrickType.Normal, Id = 1, X = 10, Y = 20 }, new BrickDto() { BrickType = BrickType.Sturdy, Id = 2, X = 40, Y = 50 } };
            levelDto.BossName = "";
            levelDto.BossUri = "";
            levelDto.BossPaths = new List<List<Point>>();
            levelDto.HasBoss = false;
            levelDto.ID = 1;

            _levelService.Setup(s => s.GetLevel(1)).Returns(levelDto);

            var level = new LevelModel(_levelService.Object);
            level.LoadLevel();

            var brick = level.Bricks[0];
            level.GameStateUpdated += OnGameStateUpdated;
            brick.Hit(1);
            level.GameStateUpdated -= OnGameStateUpdated;
            Assert.NotNull(_gameState);
            Assert.That(_gameState.NumberOfRemainingBricks, Is.EqualTo(1));
            Assert.IsTrue(_gameState.BrickWasBroke);
        }

        [Test]
        public void TestLoseGameByDamage()
        {
            var level = new LevelModel(_levelService.Object);
            level.PlayerLost += OnPlayerLost;
            level.AddScore(50);
            level.Damage(4);

            Assert.That(level.Lives, Is.EqualTo(0));
            Assert.That(_finalScore, Is.EqualTo(50));
        }

        [Test]
        public void TestLoseABall()
        {
            _resetBarPositionRequested = false;
            _ballCreationInfo = null;
            _numberOfBallCreated = 0;

            var levelDto = new LevelDto();
            levelDto.Bricks = new List<BrickDto>() { new BrickDto() { BrickType = BrickType.Normal, Id = 1, X = 10, Y = 20 }, new BrickDto() { BrickType = BrickType.Sturdy, Id = 2, X = 40, Y = 50 } };
            levelDto.BossName = "";
            levelDto.BossUri = "";
            levelDto.BossPaths = new List<List<Point>>();
            levelDto.HasBoss = false;
            levelDto.ID = 1;

            _levelService.Setup(s => s.GetLevel(1)).Returns(levelDto);

            var level = new LevelModel(_levelService.Object);
            level.OnBallCreated += OnBallCreated;
            level.GameStateUpdated += OnGameStateUpdated;
            level.ResetBarPosition += OnResetBarPosition;
            level.LoadLevel();

            Assert.NotNull(_ballCreationInfo);
            Assert.That(_ballCreationInfo.ID, Is.EqualTo(1));
            Assert.That(_ballCreationInfo.Position.X, Is.EqualTo(0));
            Assert.That(_ballCreationInfo.Position.Y, Is.EqualTo(0));
            Assert.That(_ballCreationInfo.InitialVelocity.X, Is.EqualTo(0));
            Assert.That(_ballCreationInfo.InitialVelocity.Y, Is.EqualTo(0));
            Assert.That(_numberOfBallCreated, Is.EqualTo(1));

            _numberOfBallCreated = 0;
            _ballCreationInfo = null;
            var ball = level.Balls[0];
            ball.Destroy();

            level.OnBallCreated -= OnBallCreated;
            level.GameStateUpdated -= OnGameStateUpdated;
            level.ResetBarPosition -= OnResetBarPosition;

            Assert.That(level.Lives, Is.EqualTo(2));
            Assert.IsTrue(_resetBarPositionRequested);
            Assert.That(_numberOfBallCreated, Is.EqualTo(1));
            Assert.NotNull(_ballCreationInfo);
            Assert.That(_ballCreationInfo.ID, Is.EqualTo(1));
            Assert.That(_ballCreationInfo.Position.X, Is.EqualTo(0));
            Assert.That(_ballCreationInfo.Position.Y, Is.EqualTo(0));
            Assert.That(_ballCreationInfo.InitialVelocity.X, Is.EqualTo(0));
            Assert.That(_ballCreationInfo.InitialVelocity.Y, Is.EqualTo(0));
        }

        [Test]
        public void TestLoseByLostBall()
        {
            var levelDto = new LevelDto();
            levelDto.Bricks = new List<BrickDto>() { new BrickDto() { BrickType = BrickType.Normal, Id = 1, X = 10, Y = 20 }, new BrickDto() { BrickType = BrickType.Sturdy, Id = 2, X = 40, Y = 50 } };
            levelDto.BossName = "";
            levelDto.BossUri = "";
            levelDto.BossPaths = new List<List<Point>>();
            levelDto.HasBoss = false;
            levelDto.ID = 1;

            _levelService.Setup(s => s.GetLevel(1)).Returns(levelDto);

            var level = new LevelModel(_levelService.Object);
            level.LoadLevel();
            level.PlayerLost += OnPlayerLost;
            level.AddScore(75);
            level.Balls[0].Destroy();
            level.Balls[0].Destroy();
            level.Balls[0].Destroy();

            level.PlayerLost -= OnPlayerLost;

            Assert.That(level.Lives, Is.EqualTo(0));
            Assert.That(_finalScore, Is.EqualTo(75));
        }

        [Test]
        public void TestLevelWin()
        {
            _numberOfBallCreated = 0;
            _resetBarPositionRequested = false;

            var level1Dto = new LevelDto();
            level1Dto.Bricks = new List<BrickDto>() { new BrickDto() { BrickType = BrickType.Normal, Id = 1, X = 10, Y = 20 }, new BrickDto() { BrickType = BrickType.Sturdy, Id = 2, X = 40, Y = 50 } };
            level1Dto.BossName = "";
            level1Dto.BossUri = "";
            level1Dto.BossPaths = new List<List<Point>>();
            level1Dto.HasBoss = false;
            level1Dto.ID = 1;

            var level2Dto = new LevelDto();
            level2Dto.Bricks = new List<BrickDto>() { new BrickDto() { BrickType = BrickType.Normal, Id = 1, X = 10, Y = 20 } };

            _levelService.Setup(s => s.GetLevel(1)).Returns(level1Dto);
            _levelService.Setup(s => s.GetLevel(2)).Returns(level2Dto);

            var level = new LevelModel(_levelService.Object);
            level.LoadLevel();

            level.ResetBarPosition += OnResetBarPosition;
            level.OnBallCreated += OnBallCreated;
            level.LevelEnded += OnLevelEnded;

            level.Bricks[0].Hit(0);
            level.Bricks[1].Hit(0);
            Thread.Sleep(30);
            level.Bricks[1].Hit(0);

            level.ResetBarPosition -= OnResetBarPosition;
            level.OnBallCreated -= OnBallCreated;
            level.LevelEnded -= OnLevelEnded;

            Assert.That(level.CurrentStage, Is.EqualTo("2"));
            Assert.That(_resetBarPositionRequested, Is.True);
            Assert.That(_numberOfBallCreated, Is.EqualTo(1));

            Assert.That(level.Balls.Count, Is.EqualTo(1));

            level.GameStateUpdated += OnGameStateUpdated;
            level.Balls[0].Destroy();
            Assert.That(_gameState.Lives, Is.EqualTo(2));

        }

        private void OnLevelEnded(object? sender, EventArgs e)
        {
            _levelEnded = true;
        }

        private void OnResetBarPosition(object? sender, EventArgs e)
        {
            _resetBarPositionRequested = true;
        }

        private void OnBallCreated(object? sender, BallCreationInfo ballCreationInfo)
        {
            _ballCreationInfo = ballCreationInfo;
            _numberOfBallCreated++;
        }

        private void OnPlayerLost(object? sender, int finalScore)
        {
            _finalScore = finalScore;
        }

        private void OnGameStateUpdated(object? sender, GameState gameState)
        {
            _gameState = gameState;
        }
    }
}
