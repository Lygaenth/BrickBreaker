namespace Casse_brique.DAL.API
{
    /// <summary>
    /// Dal for scores
    /// </summary>
    public interface IHighScoreDal
    {
        List<ScoreDto> GetHighScores(int numberToDisplay);

        ScoreDto GetUserRanking(int userId);

        int UpdateUserScore(ScoreDto score);
    }
}
