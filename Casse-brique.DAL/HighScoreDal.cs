using Casse_brique.DAL.API;
using Casse_brique.Domain.Scoring;
using System.Xml.Serialization;
using static System.Formats.Asn1.AsnWriter;

namespace Casse_brique.DAL
{
    public class HighScoreDal : IHighScoreDal
    {
        private const string _file = "./HighScore.Xml";

        public List<ScoreDto> GetHighScores(int numberToDisplay)
        {
            IEnumerable<ScoreDto> scores = ReadFile().Select(s => new ScoreDto() { ID = s.UserID, UserName = s.UserName, Score = s.Points, Rank = s.Rank }).OrderByDescending(s => s.Score);
            if (scores.Count() > numberToDisplay)
                scores = scores.SkipLast(scores.Count() - numberToDisplay);
            return scores.ToList();
        }

        public ScoreDto GetUserRanking(int userID)
        {
            var scores = ReadFile();
            if (scores.Any(s => s.UserID == userID))
            {
                var score = scores.First(s => s.UserID == userID);
                return new ScoreDto() { ID = score.UserID, UserName = score.UserName, Score = score.Points };
            }
            return new ScoreDto() { ID = userID, UserName = "Unranked", Score = 0 };
        }


        public async Task<int> UpdateUserScore(ScoreDto score)
        {
            var scores = ReadFile();
            UpdateRanks(scores, score.ID, score.Score);
            var rank = GetRankForNewScore(scores, score.Score);

            if (scores.Any(s => s.UserID == score.ID))
            {
                var storedScore = scores.First(s => s.UserID == score.ID);
                storedScore.Points = score.Score;
                storedScore.Rank = rank;
                return storedScore.Rank;
            }
            else
            {
                if (score.ID == 0)
                {
                    if (scores.Count == 0)
                        score.ID = 1;
                    else
                        score.ID = scores.OrderBy(s => s.UserID).Last().UserID + 1;
                }
                var newScore = new Score(score.ID, score.UserName, score.Score);
                newScore.Rank = rank;
                if (scores.Count == 0)
                    scores.Add(newScore);
                else
                    scores.Insert(rank, newScore);
            }
            await SaveScores(scores);
            return rank;
        }

        private void UpdateRanks(List<Score> scores, int userID, int score)
        {
            foreach (var s in scores.Where(s => s.Points <= score))
            {
                if (s.UserID != userID)
                    s.Rank++;
            }
        }

        private int GetRankForNewScore(List<Score> scores, int score)
        {
            if (!scores.Any(s => s.Points >= score))
                return 1;

            return scores.Last(s => s.Points >= score).Rank+1;
        }

        private async Task SaveScores(List<Score> scores)
        {
            var writer = new StreamWriter(_file);
            XmlSerializer serializer = new XmlSerializer(typeof(List<Score>));
            await Task.Run(() => serializer.Serialize(writer, scores));
            writer.Close();
        }

        private List<Score> ReadFile()
        {
            var reader = new StreamReader(_file);
            XmlSerializer serializer = new XmlSerializer(typeof(List<Score>));
            var result = serializer.Deserialize(reader) as List<Score>;
            reader.Close();
            return result == null ? new List<Score>() : result;
        }
    }
}
