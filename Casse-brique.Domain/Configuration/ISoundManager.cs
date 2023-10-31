namespace Casse_brique.Domain.Configuration
{
    public interface ISoundManager
    {
        bool SoundOn { get; }
        float VolumeDB { get; }
        int VolumePercent { get; }

        void SetSoundOnOff(bool soundOn);
        void SetVolumePerCent(int volume);
    }
}