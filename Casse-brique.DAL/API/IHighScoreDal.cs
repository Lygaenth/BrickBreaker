namespace Casse_brique.DAL.API
{
    /// <summary>
    /// Dal for scores
    /// </summary>
    public interface IHighScoreDal
    {
        /// <summary>
        /// Get high scores
        /// </summary>
        /// <param name="numberToDisplay"></param>
        /// <returns></returns>
        List<ScoreDto> GetHighScores(int numberToDisplay);

        /// <summary>
        /// Get user ranking
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        ScoreDto GetUserRanking(int userId);

        /// <summary>
        /// Update user score
        /// </summary>
        /// <param name="score"></param>
        /// <returns></returns>
        Task<int> UpdateUserScore(ScoreDto score);
    }
}
