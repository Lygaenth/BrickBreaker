using Newtonsoft.Json;

namespace Casse_brique.Domain.Scoring
{
    /// <summary>
    /// Score
    /// </summary>
    public class Score
    {
        /// <summary>
        /// UserID
        /// </summary>
        public int UserID { get; private set; }

        /// <summary>
        /// User name
        /// </summary>
        public string UserName { get; private set; }

        /// <summary>
        /// Points
        /// </summary>
        public int Points { get; private set; }

        /// <summary>
        /// Ranking of player's best score
        /// </summary>
        public int Rank { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="userName"></param>
        /// <param name="points"></param>
        public Score(int userID, string userName, int points)
        {
            UserID = userID;
            UserName = userName;
            Points = points;
        }
    }
}
