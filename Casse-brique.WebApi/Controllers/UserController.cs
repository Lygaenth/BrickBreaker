using Casse_brique.Domain.Scoring;
using Microsoft.AspNetCore.Mvc;

namespace Casse_brique.WebApi.Controllers
{
    /// <summary>
    /// User controller for users (data should be managed through a database)
    /// </summary>
    [ApiController]
    [Route("Users")]
    public class UserController : ControllerBase
    {
        private readonly Dictionary<int, Score> _usersBestScores;

        public UserController()
        {
            _usersBestScores = new Dictionary<int, Score>();
            _usersBestScores[1] = new Score(1, "Test1", 10000);
            _usersBestScores[2] = new Score(2, "Test2", 9000);
            _usersBestScores[3] = new Score(3, "Test3", 7000);
        }

        /// <summary>
        /// Sending data back as dictionary or else Godot cannot parse it as a dictionary
        /// </summary>
        /// <returns></returns>
        [HttpGet("Highscores")]
        public Dictionary<int, Score> GetHighScores()
        {
            IEnumerable<Score> orderedScores = _usersBestScores.Values.OrderByDescending(s => s.Points);
            if (orderedScores.Count() > 10)
                orderedScores = orderedScores.SkipLast(_usersBestScores.Count - 10);
            var scoreList = orderedScores.ToList();
            var results = new Dictionary<int, Score>();
            for (int i = 0; i < Math.Min(scoreList.Count, 10); i++)
            {
                scoreList[i].Rank = i + 1;
                results.Add(i, scoreList[i]);
            }
            return results;
        }

        [HttpPost("Users/{id}/Score")]
        public async Task PostUserScore(Score score)
        {
            _usersBestScores[score.Rank] = score;
        }
    }
}
