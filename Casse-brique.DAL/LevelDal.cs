using Casse_brique.DAL.API;
using Casse_brique.Domain.API;
using Newtonsoft.Json;

namespace Casse_brique.DAL
{
    public class LevelDal : ILevelDal
    {
        private readonly List<LevelDto> _levels;

        /// <summary>
        /// Constructor loading levels
        /// </summary>
        public LevelDal()
        {
            _levels = new List<LevelDto>();
            var textReader = new StreamReader("level.json");
            var levels = JsonConvert.DeserializeObject<List<LevelDto>>(textReader.ReadToEnd());
            if (levels != null)
                _levels.AddRange(levels);
            textReader.Close();
        }

        /// <summary>
        /// Has level
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool HasLevel(int id)
        {
            return _levels.Any(l => l.ID == id);
        }

        /// <summary>
        /// Get level
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public LevelDto GetLevel(int id)
        {
            if (HasLevel(id))
                return _levels.First(l => l.ID == id);

            return _levels[0];
        }

        /// <summary>
        /// Get level count
        /// </summary>
        /// <returns></returns>
        public int GetLevelsCount()
        {
            return _levels.Count;
        }

        /// <summary>
        /// Save levels (not useful for now but might be useful to add levels thourgh an editor) 
        /// </summary>
        public void Save()
        {            
            var textWriter = new StreamWriter("level.json");
            var serializedLevel = JsonConvert.SerializeObject(_levels);
            textWriter.Write(serializedLevel);
            textWriter.Close();
        }
    }
}
