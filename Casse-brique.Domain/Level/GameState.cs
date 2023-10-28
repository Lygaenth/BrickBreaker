namespace Casse_brique.Domain.Level
{
    public class GameState
    {
        public int Score { get; }
        public int Lives { get; }
        public bool BrickWasBroke { get; }
        public int NumberOfRemainingBricks { get; }

        public GameState(int score, int lives, int numberOfRemainingBricks,  bool brickWasBroke)
        {
            Score = score;
            Lives = lives;
            NumberOfRemainingBricks = numberOfRemainingBricks;
            BrickWasBroke = brickWasBroke;
        }
    }
}
