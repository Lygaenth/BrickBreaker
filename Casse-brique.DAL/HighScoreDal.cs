using Casse_brique.DAL.API;
using Casse_brique.Domain.Scoring;
using System.Xml.Serialization;
using static System.Formats.Asn1.AsnWriter;

namespace Casse_brique.DAL
{
    public class HighScoreDal : IHighScoreDal
    {
        private const string _file = "./HighScore.Xml";

        private List<Score> _scores;

        public HighScoreDal()
        {
            _scores = new List<Score>();
            //_scores.Add(new Score(1, "Bob", 1000));
            //_scores.Add(new Score(2, "Ted", 900));
            //_scores.Add(new Score(3, "NomBeaucoupTropLong", 800));
            //_scores.Add(new Score(4, "Nyx", 700));
            //_scores.Add(new Score(5, "Zagreus", 600));
            //_scores.Add(new Score(6, "Hades", 500));
            //_scores.Add(new Score(7, "Artemis", 400));
            //_scores.Add(new Score(8, "Athena", 300));
        }

        public List<ScoreDto> GetHighScores(int numberToDisplay)
        {
            IEnumerable<ScoreDto> scores = _scores.Select(s => new ScoreDto() { ID = s.UserID, UserName = s.UserName, Score = s.Points }).OrderByDescending(s => s.Score);
            if (scores.Count() > numberToDisplay)
                scores = scores.SkipLast(scores.Count() - numberToDisplay);
            return scores.ToList();
        }

        public ScoreDto GetUserRanking(int userID)
        {
            if (_scores.Any(s => s.UserID == userID))
            {
                var score = _scores.First(s => s.UserID == userID);
                return new ScoreDto() { ID = score.UserID, UserName = score.UserName, Score = score.Points };
            }
            return new ScoreDto() { ID = userID, UserName = "", Score = 0 };
        }

        public int UpdateUserScore(int userID, int score)
        {
            UpdateRanks(userID, score);
            var rank = GetRankForNewScore(score);

            if (_scores.Any(s => s.UserID == userID))
            {
                var storedScore = _scores.First(s => s.UserID == userID);
                storedScore.Points = score;
                storedScore.Rank = rank;
                return storedScore.Rank;
            }
            else
            {
                _scores.Insert(rank, new Score(userID, "undefined", score));
            }
            SaveFile();
            return rank;
        }

        private void UpdateRanks(int userID, int score)
        {
            foreach (var s in _scores.Where(s => s.Points >= score))
            {
                if (s.UserID != userID)
                    s.Rank++;
            }
        }

        private int GetRankForNewScore(int score)
        {
            if (!_scores.Any(s => s.Points <= score))
                return 1;

            return _scores.Last(s => s.Points <= score).Rank+1;
        }

        private void SaveFile()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Score>));
            serializer.Serialize(new StreamWriter(_file), _scores);
        }
    }
}
