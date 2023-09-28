using Casse_brique.DAL;
using Casse_brique.DAL.API;
using Casse_brique.Domain.API;
using Casse_brique.Domain.Scoring;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cassebrique.Services
{
    public class LocalHighScoreService : IHighScoreService
    {
        private readonly IHighScoreDal _highScoreDal;

        public event IHighScoreService.HighScoreEventHandler OnHighScoreResult;

        public LocalHighScoreService(IHighScoreDal highScoreDal) 
        {
            _highScoreDal = highScoreDal;
        }

        public void PostScore(Score score)
        {
            _highScoreDal.UpdateUserScore(new ScoreDto { ID = score.UserID, UserName = score.UserName, Score = score.Points });
        }

        public void RequestHighScores()
        {
            var scores = _highScoreDal.GetHighScores(10);
            
            if (OnHighScoreResult != null)
                OnHighScoreResult(scores.ConvertAll<Score>(s =>
                {
                    var score =new Score(s.ID, s.UserName, s.Score);
                    score.Rank = s.Rank;
                    return score;
                }));
        }
    }
}
