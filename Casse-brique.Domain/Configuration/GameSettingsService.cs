namespace Casse_brique.Domain.Configuration
{
    public class GameSettingsService
    {
        private readonly ISoundManager _soundManager;

        public GameSettingsService(ISoundManager soundManager)
        {
            _soundManager = soundManager;
        }

        public SoundManager GetSoundManager()
        {
            return _soundManager;
        }
    }
}
