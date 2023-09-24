﻿using Cassebrique.Domain.API;
using Cassebrique.Domain.Bricks;
using Godot;
using System;
using System.Collections.Generic;

namespace Cassebrique.Services
{
    /// <summary>
    /// Service providing level structure
    /// </summary>
    public class LevelService : ILevelService
    {
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
    }
}
