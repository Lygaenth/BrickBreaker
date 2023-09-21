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
        private List<Score> _scores;

        public event IHighScoreService.HighScoreEventHandler OnHighScoreResult;

        public LocalHighScoreService() 
        {
            _scores = new List<Score>();
            _scores.Add(new Score(1, "Bob", 1000));
            _scores.Add(new Score(2, "Ted", 900));
            _scores.Add(new Score(3, "NomBeaucoupTropLong", 800));
            _scores.Add(new Score(4, "Nyx", 700));
            _scores.Add(new Score(5, "Zagreus", 600));
            _scores.Add(new Score(6, "Hades", 500));
            _scores.Add(new Score(7, "Artemis", 400));
            _scores.Add(new Score(8, "Athena", 300));
        }

        public void PostScore(Score score)
        {
            _scores.Add(score);
            _scores = _scores.OrderByDescending( s => s.Points).ToList();
        }

        public void RequestHighScores()
        {
            var scores = _scores.OrderByDescending(s => s.Points).ToList();
            if (scores.Count > 10)
                scores = scores.SkipLast(scores.Count - 10).ToList();
            for (int i = 0; i < scores.Count; i++)
                scores[i].Rank = i + 1;
            if (OnHighScoreResult != null)
                OnHighScoreResult(scores);
        }
    }
}
