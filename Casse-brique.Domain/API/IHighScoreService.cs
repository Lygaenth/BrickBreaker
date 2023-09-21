using Casse_brique.Domain.Scoring;

namespace Casse_brique.Domain.API
{
    /// <summary>
    /// API for High score service
    /// </summary>
    public interface IHighScoreService
    {
        /// <summary>
        /// High score event Handler
        /// </summary>
        /// <param name="scores"></param>
        public delegate void HighScoreEventHandler(List<Score> scores);

        /// <summary>
        /// High score published event
        /// </summary>
        public event HighScoreEventHandler OnHighScoreResult;

        /// <summary>
        /// Request High scores
        /// </summary>
        public void RequestHighScores();

        /// <summary>
        /// Post score
        /// </summary>
        /// <param name="score"></param>
        public void PostScore(Score score);
    }
}
