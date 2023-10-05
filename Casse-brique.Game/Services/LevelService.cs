using Casse_brique.DAL.API;
using Casse_brique.Domain.API;
using Cassebrique.Domain.API;
using Cassebrique.Domain.Bricks;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Cassebrique.Services
{
    /// <summary>
    /// Service providing level structure
    /// </summary>
    public class LevelService : ILevelService
    {
        private readonly ILevelDal _levelDal;

        public LevelService(ILevelDal levelDal)
        {
            _levelDal = levelDal;
        }

        public LevelDto GetLevel(int levelId)
        {
            if (_levelDal.HasLevel(levelId))
                return _levelDal.GetLevel(levelId);
            
            var level = new LevelDto();
            level.Bricks = GetRandomBricks(levelId);
            return level;
        }

        private List<BrickDto> GetRandomBricks(int levelId)
        {
            var seed = DateTime.Now.Millisecond;
            var rand = new Random(levelId);

            var bricks = new List<BrickDto>();

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    var brick = new BrickDto();
                    brick.Id = i * j + j;
                    brick.X = i * 100 + 150;
                    brick.Y = j * 52 + 81;
                        brick.BrickType = (BrickType) (rand.Next() %6);
                    bricks.Add(brick);
                }
            }
            return bricks;
        }

    }
}
