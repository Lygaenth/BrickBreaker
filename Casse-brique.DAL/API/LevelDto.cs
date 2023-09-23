using Cassebrique.Domain.Bricks;

namespace Casse_brique.DAL.API
{
    public class LevelDto
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public List<BrickDto> Bricks { get; set; }

        public LevelDto()
        {
            Name = "";
            Bricks = new List<BrickDto>();
        }
    }
}
