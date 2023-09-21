using Cassebrique.Domain.Bricks;

namespace Cassebrique.Domain.API
{
    /// <summary>
    /// Interface for level service building
    /// </summary>
    public interface ILevelService
    {
        /// <summary>
        /// Get level bricks
        /// </summary>
        /// <param name="levelId"></param>
        /// <returns></returns>
        public List<BrickDto> GetBricks(int levelId);        
    }
}
