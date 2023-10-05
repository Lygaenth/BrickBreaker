using Casse_brique.Domain.API;

namespace Casse_brique.DAL.API
{
    /// <summary>
    /// Level DAL
    /// </summary>
    public interface ILevelDal
    {
        /// <summary>
        /// Get level by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        LevelDto GetLevel(int id);

        /// <summary>
        /// Has level for this id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool HasLevel(int id);

        /// <summary>
        /// Return number of existing levels
        /// </summary>
        /// <returns></returns>
        int GetLevelsCount();
    }
}
