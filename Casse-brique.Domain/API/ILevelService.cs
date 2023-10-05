using Casse_brique.Domain.API;
using Cassebrique.Domain.Bricks;

namespace Cassebrique.Domain.API
{
    /// <summary>
    /// Interface for level service building
    /// </summary>
    public interface ILevelService
    {
        /// <summary>
        /// Get level description
        /// </summary>
        /// <param name="levelId"></param>
        /// <returns></returns>
        LevelDto GetLevel(int levelId);      
    }
}
