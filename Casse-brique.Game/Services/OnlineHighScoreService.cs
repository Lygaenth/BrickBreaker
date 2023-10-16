using Casse_brique.Domain.API;
using Casse_brique.Domain.Scoring;
using Cassebrique.Exceptions;
using Cassebrique.Locators;
using Cassebrique.Services;
using Godot;
using Godot.Collections;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Casse_brique.Services
{
    /// <summary>
    /// Services for exchange with high score controller
    /// </summary>
    public class OnlineHighScoreService : IHighScoreService
    {
        private readonly IAutoLoaderProvider _autoLoaderProvider;
        private readonly IAuthenticationTokenService _tokenManager;

        public event IHighScoreService.HighScoreEventHandler OnHighScoreResult;
        private HttpRequest _requestNode;
        public delegate void HighScoreListResultEventHandler();


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="requestNode"></param>
        public OnlineHighScoreService(IAutoLoaderProvider autoLoaderProvider, IAuthenticationTokenService tokenManager)
        {
            _autoLoaderProvider = autoLoaderProvider;
            _tokenManager = tokenManager;
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
            SetRequestNode();
            if (needsResult)
                _requestNode.RequestCompleted += OnRequestCompleted;
            var token = _tokenManager.GetToken();
            var error = _requestNode.Request(url, customHeaders: new string[] { "Authorization: Bearer " + token });
            GD.Print("Request error: "+error+" token: "+token);
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

            GD.Print("Request responseCode: "+responseCode);

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
            SetRequestNode();
            string json = JsonConvert.SerializeObject(score);
            string[] headers = new string[] { "Content-Type: application/json" };
            
            _requestNode.Request("https://localhost:7253/Users/"+score.UserID+"/Score", headers, HttpClient.Method.Post, json);
        }

        private void SetRequestNode()
        {
            if (_requestNode == null)
                _requestNode = _autoLoaderProvider.GetHttopRequestNode();
        }
    }
}
