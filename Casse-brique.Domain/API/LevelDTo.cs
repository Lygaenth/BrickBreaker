using Cassebrique.Domain.Bricks;
using System.Drawing;

namespace Casse_brique.Domain.API
{
    public class LevelDto
    {
        public int ID { get; set; }
        public bool HasBoss { get; set; }
        public string BossUri { get; set; }
        public List<BrickDto> Bricks { get; set; }
        public List<Point> BossPath { get; set; }

        public LevelDto()
        {
            BossUri = string.Empty;
            Bricks = new List<BrickDto>();
            BossPath = new List<Point>();
        }
    }
}
