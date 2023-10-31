using Casse_brique.Domain.Bricks;
using Casse_brique.Domain.Enums;
using Cassebrique.Domain.API;
using Cassebrique.Domain.Bricks;
using Godot;

namespace Casse_brique.Domain.Level
{
    public class LevelModel
    {
        private int _lives = 3;
        public int Lives { get => _lives; }

        private int _score = 0;
        public int Score { get => _score; }

        private int _numberOfBricks = 0;
        private int _numberOfBallsCreated = 0;

        private int _currentStage = 1;
        public string CurrentStage { get => _currentStage.ToString(); }

        private LevelObjective _levelObjective;

        public int GetRemainingLevelResource()
        {
            if (_bossModel != null)
                return _bossModel.HP;
            else
                return _numberOfBricks;
        }

        private List<BrickModel> _brickModels;
        public IReadOnlyList<BrickModel> Bricks { get => _brickModels; }

        private List<BallModel> _ballModels;
        public IReadOnlyList<BallModel> Balls { get => _ballModels; }

        private BossModel _bossModel;
        public BossModel? Boss { get => _bossModel; }

        private readonly ILevelService _levelService;

        #region Events
        public event EventHandler<GameState> GameStateUpdated;

        public event EventHandler<EventArgs> LevelEnded;

        public event EventHandler<LevelObjective> LevelLoaded;

        public event EventHandler<EventArgs> ResetBarPosition;

        public event EventHandler<int> PlayerLost;

        public event EventHandler<BallCreationInfo> OnBallCreated;

        public event EventHandler<EventArgs> OnAnyBounce;
        #endregion

        public LevelModel(ILevelService levelService, int startLevel)
        {
            _currentStage = startLevel;
            _levelService = levelService;
            _brickModels = new List<BrickModel>();
            _ballModels = new List<BallModel>();
        }

        public void LoadLevel()
        {
            _brickModels.Clear();
            _bossModel = null;
            var level = _levelService.GetLevel(_currentStage);
            foreach (var brick in level.Bricks)
            {
                var position = new Vector2(brick.X, brick.Y);
                var brickModel = brick.BrickType switch
                {
                    BrickType.Normal => new BrickModel(position),
                    BrickType.Sturdy => new SturdyBrickModel(position),
                    BrickType.TrulySturdy => new TrulySturdyBrickModel(position),
                    BrickType.Unbreakable => new UnbreakableBrickModel(position),
                    BrickType.Accelerator => new AcceleratorBrickModel(position),
                    BrickType.Divider => new DividerBrickModel(position),
                    _ => throw new Exception("Unvalid brick type")
                };
                _brickModels.Add(brickModel);
                brickModel.Broken += OnBrickBroken;
            }
            _numberOfBricks = _brickModels.Where(b => b.BrickType != BrickType.Unbreakable).Count();

            _levelObjective = LevelObjective.Bricks;

            if (level.HasBoss)
            {
                _levelObjective = LevelObjective.Boss;
                _bossModel = new BossModel(level.BossName, 10, level.BossPaths);
                _bossModel.Destroyed += OnBossDestroyed;
            }

            LevelLoaded?.Invoke(this, _levelObjective);
            ResetBarPosition?.Invoke(this, new EventArgs());

            CreateBall(new BallCreationInfo(1, new Vector2(), new Vector2()));
            GameStateUpdated?.Invoke(this, CreateGameState());
        }

        void CreateBall(BallCreationInfo creationInfo)
        {
            _numberOfBallsCreated++;
            var ball = new BallModel(_numberOfBallsCreated);
            SubscribeToBall(ball);
            _ballModels.Add(ball);

            OnBallCreated?.Invoke(this, new BallCreationInfo(ball.ID, creationInfo.Position, creationInfo.InitialVelocity));
        }

        private void OnBallBounced(object? sender, int e)
        {
            OnAnyBounce?.Invoke(this, new EventArgs());
        }

        private void OnBallDuplicated(object? sender, BallCreationInfo e)
        {
            _numberOfBallsCreated++;
            CreateBall(e);
        }

        private void OnBallDestroyed(object? sender, int id)
        {
            var destroyedBall = _ballModels.First(b => b.ID == id);
            UnSubscribeFromBall(destroyedBall);
            _ballModels.Remove(destroyedBall);
            if (_ballModels.Count > 0)
                return;

            _lives--;
            GameStateUpdated?.Invoke(this, CreateGameState());
            if (!CheckGameIsLost())
            {
                CleanUpBalls();

                CreateBall(new BallCreationInfo(1, new Vector2(), new Vector2()));
                ResetBarPosition?.Invoke(this, new EventArgs());
            }
        }

        private void CleanUpBalls()
        {
            _numberOfBallsCreated = 0;
            while (_ballModels.Count > 0)
            {
                UnSubscribeFromBall(_ballModels[0]);
                _ballModels[0].Destroy();
                _ballModels.RemoveAt(0);
            }
        }

        private void SubscribeToBall(BallModel ball)
        {
            ball.Destroyed += OnBallDestroyed;
            ball.Duplicated += OnBallDuplicated;
            ball.Bounced += OnBallBounced;
        }

        private void UnSubscribeFromBall(BallModel ball)
        {
            ball.Destroyed -= OnBallDestroyed;
            ball.Duplicated -= OnBallDuplicated;
            ball.Bounced -= OnBallBounced;
        }

        private void OnBrickBroken(object? sender, int score)
        {
            _numberOfBricks--;
            _score += score;
            GameStateUpdated?.Invoke(this, CreateGameState());
            if (_numberOfBricks == 0)
                EndLevel();
        }

        private void OnBossDestroyed(object? sender, int score)
        {
            _score += score;
            EndLevel();
        }

        public void AddScore(int score)
        {
            _score += score;
            GameStateUpdated?.Invoke(this, CreateGameState());
        }

        public void Damage(int damage)
        {
            _lives = Mathf.Clamp(_lives - damage, 0, 10);
            GameStateUpdated?.Invoke(this, CreateGameState());
            CheckGameIsLost();
        }

        public void EndLevel()
        {
            LevelEnded?.Invoke(this, new EventArgs());

            if (_ballModels.Count > 1)
                _score += _ballModels.Count * 100;

            CleanUpBalls();
            foreach (var brick in Bricks.Where(b => b.BrickType == BrickType.Unbreakable).ToList())
                brick.Destroy(0);

            _currentStage++;

            LoadLevel();
        }

        private GameState CreateGameState()
        {
            return new GameState(_score, _lives, GetRemainingLevelResource(), true);
        }

        private bool CheckGameIsLost()
        {
            if (_lives <= 0)
            {
                PlayerLost?.Invoke(this, _score);
                return true;
            }

            return false;
        }
    }
}
