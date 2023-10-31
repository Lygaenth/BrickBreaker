namespace Casse_brique.Domain.Configuration
{
    public class SoundSettingsDto
    {
        public bool SoundOn { get; }
        public int VolumePercent { get; }

        public SoundSettingsDto(bool soundOn, int volumePerCent)
        {
            SoundOn = soundOn;
            VolumePercent = volumePerCent;
        }
    }
}
