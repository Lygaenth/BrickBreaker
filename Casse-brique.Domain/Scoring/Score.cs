using Newtonsoft.Json;
using System.Xml.Serialization;

namespace Casse_brique.Domain.Scoring
{
    [XmlRootAttribute("Score")]
    /// <summary>
    /// Score
    /// </summary>
    public class Score
    {
        [XmlElement]
        /// <summary>
        /// UserID
        /// </summary>
        public int UserID { get; private set; }

        [XmlElement]
        /// <summary>
        /// User name
        /// </summary>
        public string UserName { get; private set; }

        [XmlElement]
        /// <summary>
        /// Points
        /// </summary>
        public int Points { get; set; }

        [XmlElement]
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
