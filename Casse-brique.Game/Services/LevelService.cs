using Casse_brique.Domain.API;
using Cassebrique.Domain.API;
using Cassebrique.Domain.Bricks;
using Godot;
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
        public LevelDto GetLevel(int levelId)
        {
            switch (levelId)
            {
                case 1:
                    return CreateFirstLevel();
                case 2:
                    return CreateSecondLevel();
                case 4:
                    return CreateMozartLevel();
                default:
                    var level = new LevelDto();
                    level.Bricks = GetBricks(levelId);
                    return level;
            }
        }

        public List<BrickDto> GetBricks(int levelId)
        {
            var seed = DateTime.Now.Millisecond;
            var rand = new Random(seed);

            var bricks = new List<BrickDto>();

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    var brick = new BrickDto();
                    brick.Id = i * j + j;
                    brick.X = i * 100 + 150;
                    brick.Y = j * 52 + 81;
                    //if (levelId == 1)
                    //    brick.BrickType = BrickType.Normal;
                    //if (levelId == 2)
                    //    brick.BrickType = (j % 2 == 0) ? BrickType.Normal : BrickType.Divider;
                    //if (levelId >= 3)
                        brick.BrickType = (BrickType) (rand.Next() %6);
                    bricks.Add(brick);
                    GD.Print(brick.BrickType);
                }
            }
            return bricks;
        }

        private LevelDto CreateFirstLevel()
        {
            var firstLevel = new LevelDto();
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    var brick = new BrickDto();
                    brick.Id = i * j + j;
                    brick.X = i * 100 + 150;
                    brick.Y = j * 52 + 81;
                    brick.BrickType = BrickType.Normal;
                    firstLevel.Bricks.Add(brick);
                }
            }
            return firstLevel;
        }

        private LevelDto CreateSecondLevel()
        {
            var firstLevel = new LevelDto();
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    var brick = new BrickDto();
                    brick.Id = i * j + j;
                    brick.X = i * 100 + 150;
                    brick.Y = j * 52 + 81;
                    brick.BrickType = (j % 2 == 0) ? BrickType.Normal : BrickType.Divider;
                    firstLevel.Bricks.Add(brick);
                }
            }
            return firstLevel;
        }

        private LevelDto CreateMozartLevel()
        {
            var level = new LevelDto();
            var brick1 = new BrickDto();
            brick1.Id = 1;
            brick1.X = 100;
            brick1.Y = 100;
            brick1.BrickType = BrickType.Unbreakable;
            level.Bricks.Add(brick1);

            var brick2 = new BrickDto();
            brick2.Id = 2;
            brick2.X = 900;
            brick2.Y = 100;
            brick2.BrickType = BrickType.Unbreakable;
            level.Bricks.Add(brick2);

            var brick3 = new BrickDto();
            brick3.Id = 3;
            brick3.X = 100;
            brick3.Y = 400;
            brick3.BrickType = BrickType.Unbreakable;
            level.Bricks.Add(brick3);

            var brick4 = new BrickDto();
            brick4.Id = 4;
            brick4.X = 900;
            brick4.Y = 400;
            brick4.BrickType = BrickType.Unbreakable;
            level.Bricks.Add(brick4);

            level.HasBoss = true;
            level.BossUri = "res://Scenes/GamePlay/Bosses/BossMozart.tscn";
            level.BossPath.Add(new Point(250, 150));
            level.BossPath.Add(new Point(750, 450));
            level.BossPath.Add(new Point(750, 150));
            level.BossPath.Add(new Point(250, 450));
            level.BossPath.Add(new Point(250, 150));
            return level;
        }
    }
}
