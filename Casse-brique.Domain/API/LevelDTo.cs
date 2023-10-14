using Cassebrique.Domain.Bricks;
using System.Drawing;

namespace Casse_brique.Domain.API
{
    /// <summary>
    /// Level Data object
    /// </summary>
    public class LevelDto
    {
        public int ID { get; set; }

        /// <summary>
        /// Has boss
        /// </summary>
        public bool HasBoss { get; set; }

        /// <summary>
        /// Boss URI
        /// </summary>
        public string BossUri { get; set; }

        /// <summary>
        /// List of bricks
        /// </summary>
        public List<BrickDto> Bricks { get; set; }

        /// <summary>
        /// Boss paths
        /// </summary>
        public List<List<Point>> BossPaths { get; set; }

        /// <summary>
        /// Boss name
        /// </summary>
        public string BossName { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public LevelDto()
        {
            BossUri = string.Empty;
            Bricks = new List<BrickDto>();
            BossPaths = new List<List<Point>>();
        }
    }
}
