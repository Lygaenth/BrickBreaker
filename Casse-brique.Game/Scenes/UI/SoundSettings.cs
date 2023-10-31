using Casse_brique.Domain.Configuration;
using Godot;

public partial class SoundSettings : GameBaseSettings
{
	private ISoundManager _soundManager;
	private SoundSettingsDto _currentSettings;

	private HSlider _slider;
	private CheckButton _checkButton;

	public void Setup(ISoundManager soundManager)
	{
		_soundManager = soundManager;
	}

	public override void _Ready()
	{
		_slider = GetNode<HSlider>("MarginContainer/VBoxContainer/GridContainer2/MarginContainer/HSlider");
		_checkButton = GetNode<CheckButton>("MarginContainer/VBoxContainer/GridContainer2/MarginContainer2/CheckButton");

		_currentSettings = new SoundSettingsDto(_soundManager.SoundOn, _soundManager.VolumePercent);
	}

	private void OnVolumeValueChanged(float value)
	{
		_soundManager.SetVolumePerCent((int)value);
		_changed = true;
	}

    private void OnSoundOnOffToggled(bool toggled)
	{
		_soundManager.SetSoundOnOff(toggled);
		_changed = true;
	}

	public override void CancelChanges()
	{
		_soundManager.SetVolumePerCent(_currentSettings.VolumePercent);
		_soundManager.SetSoundOnOff(_currentSettings.SoundOn);
	}
}
