using Casse_brique.DAL;
using Casse_brique.DAL.API;
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
        private readonly IHighScoreDal _highScoreDal;

        public UserController(IHighScoreDal highScoreDal)
        {
            _highScoreDal = highScoreDal;
        }

        /// <summary>
        /// Sending data back as dictionary or else Godot cannot parse it as a dictionary
        /// </summary>
        /// <returns></returns>
        [HttpGet("Highscores")]
        public Dictionary<int, Score> GetHighScores()
        {
            var highScores = _highScoreDal.GetHighScores(10);

            var results = new Dictionary<int, Score>();

            for (int i = 0; i < highScores.Count; i++)
            {
                var score = new Score
                {
                    Rank = highScores[i].Rank,
                    UserID = highScores[i].ID,
                    UserName = highScores[i].UserName,
                    Points = highScores[i].Score
                };
                results.Add(score.Rank, score);
            }
            return results;
        }

        [HttpPost("{id}/Score")]
        public async Task PostUserScore(Score score)
        {
            _highScoreDal.UpdateUserScore(new ScoreDto() { ID = score.UserID, UserName = score.UserName, Score = score.Points });
        }
    }
}
