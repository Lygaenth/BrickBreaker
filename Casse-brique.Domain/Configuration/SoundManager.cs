using Godot;

namespace Casse_brique.Domain.Configuration
{
    public class SoundManager : ISoundManager
    {
        private float _volumeDb = 0;
        public float VolumeDB { get => _volumeDb; }

        private int _volumePercent;
        public int VolumePercent { get => _volumePercent; }

        private bool _soundOn;
        public bool SoundOn { get => _soundOn; }

        public SoundManager()
        {
            _volumeDb = 0;
            _volumePercent = (int)(Mathf.DbToLinear(0.0f) * 100);
            _soundOn = true;
        }

        public void SetVolumePerCent(int volume)
        {
            _volumePercent = volume;
            _volumeDb = Mathf.LinearToDb((float)volume / 100);
        }

        public void SetSoundOnOff(bool soundOn)
        {
            _soundOn = soundOn;
        }
    }
}
