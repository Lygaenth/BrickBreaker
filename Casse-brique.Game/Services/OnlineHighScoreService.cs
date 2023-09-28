using Casse_brique.Domain.API;
using Casse_brique.Domain.Scoring;
using Cassebrique.Exceptions;
using Godot;
using Godot.Collections;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Casse_brique.Services
{
    /// <summary>
    /// Services for exchange with high score controller
    /// </summary>
    public class OnlineHighScoreService : IHighScoreService
    {
        private readonly HttpRequest _requestNode;

        public event IHighScoreService.HighScoreEventHandler OnHighScoreResult;

        public delegate void HighScoreListResultEventHandler();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="requestNode"></param>
        public OnlineHighScoreService(HttpRequest requestNode)
        {
            _requestNode = requestNode;
        }

        /// <summary>
        /// Request to the API service 
        /// </summary>
        public void RequestHighScores()
        {
            SendRequest("https://localhost:7253/Users/Highscores", true);
        }

        /// <summary>
        /// Send request (could be reused in abstract implementation)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="needsResult"></param>
        private void SendRequest(string url, bool needsResult = false)
        {
            if (needsResult)
                _requestNode.RequestCompleted += OnRequestCompleted;

            var error = _requestNode.Request(url);
            GD.Print(error);
            if (error != Error.Ok)
                throw new ConnectionFailedException(error);
        }

        /// <summary>
        /// On results received (could be reused in abstract implementation)
        /// </summary>
        /// <param name="result"></param>
        /// <param name="responseCode"></param>
        /// <param name="headers"></param>
        /// <param name="body"></param>
        private void OnRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
        {
            _requestNode.RequestCompleted -= OnRequestCompleted;

            var scores = ConvertResult(GetDataFromBody(body));

            if (OnHighScoreResult != null)
                OnHighScoreResult(scores);
        }

        /// <summary>
        /// Convert result from json into scores
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private List<Score> ConvertResult(Dictionary data)
        {
            var scores = new List<Score>();
            foreach (var key in data.Keys)
            {
                var scoreDto = data[key].AsGodotDictionary();
                var score =new Score(Convert.ToInt32(scoreDto["userID"].ToString()), scoreDto["userName"].ToString(), Convert.ToInt32(scoreDto["points"].ToString()));
                score.Rank = Convert.ToInt32(scoreDto["rank"].ToString());
                scores.Add(score);
            }
            return scores;
        }

        /// <summary>
        /// Extract data from body
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        private Dictionary GetDataFromBody(byte[] body)
        {
            var json = new Json();
            json.Parse(body.GetStringFromUtf8());
            return json.Data.AsGodotDictionary();
        }

        public void PostScore(Score score)
        {
            string json = JsonConvert.SerializeObject(score);
            string[] headers = new string[] { "Content-Type: application/json" };
            
            _requestNode.Request("https://localhost:7253/Users/"+score.UserID+"/Score", headers, HttpClient.Method.Post, json);
        }
    }
}
