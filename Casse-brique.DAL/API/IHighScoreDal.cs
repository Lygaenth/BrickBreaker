namespace Casse_brique.DAL.API
{
    public interface IHighScoreDal
    {
        List<ScoreDto> GetHighScores(int numberToDisplay);

        ScoreDto GetUserRanking(int userId);

        int UpdateUserScore(int userId, int score);
    }
}
