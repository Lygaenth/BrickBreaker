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
    }
}
