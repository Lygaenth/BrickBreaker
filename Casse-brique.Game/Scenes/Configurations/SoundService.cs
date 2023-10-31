using Casse_brique.Domain.Configuration;
using Godot;

public partial class SoundService : Node
{
    private SoundManager _soundManager;

    public SoundService()
    {
        _soundManager = new SoundManager();
    }

    public SoundSettingsDto GetSoundSettings()
    {
        return new SoundSettingsDto(_soundManager.SoundOn, _soundManager.VolumeDB);
    }
}
