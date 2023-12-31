﻿using Cassebrique.Domain.Bricks;

namespace Cassebrique.Factory
{
    /// <summary>
    /// Interface for brick factory
    /// </summary>
    public interface IBrickFactory
    {
        /// <summary>
        /// Create brick based on model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Brick CreateBrick(BrickDto model);
    }
}