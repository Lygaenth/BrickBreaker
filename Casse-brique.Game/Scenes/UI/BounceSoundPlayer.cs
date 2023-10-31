using Godot;

public partial class BounceSoundPlayer : AudioStreamPlayer
{
    private const float PitchRatio = 1.0595f;
    private float[] _highPitch = new float[] { 1, 1 / Mathf.Pow(PitchRatio, 4f), 1 / Mathf.Pow(PitchRatio, 7f), 1 / Mathf.Sqrt2 };// 1 / Mathf.Pow(PitchRatio, 2f), 1 / Mathf.Pow(PitchRatio, 4f), 1 / Mathf.Pow(PitchRatio, 6f), 1 / Mathf.Pow(PitchRatio, 7f), 1 / Mathf.Pow(PitchRatio, 8f), 1 / Mathf.Pow(PitchRatio, 10f), 1 / Mathf.Pow(PitchRatio, 12f) };
    private float[] _lowPitch = new float[] { 0.5f, 0.5f / Mathf.Pow(PitchRatio, 4f), 0.5f / Mathf.Pow(PitchRatio, 7f), 0.5f * Mathf.Pow(PitchRatio, 4f), 0.5f / Mathf.Sqrt2, 0.25f, 0.25f * Mathf.Pow(PitchRatio, 4f), };
    private int _lowPitchIndex = 0;
    private int _highPitchIndex = 0;

    private SoundService _soundService;

    public override void _Ready()
    {
        _soundService = GetNode<SoundService>("/root/SoundService");
    }

    public void Play(bool isHeavy)
	{
        PitchScale = isHeavy ? _lowPitch[_lowPitchIndex] : _highPitch[_highPitchIndex];
        if (isHeavy)
            _lowPitchIndex = (_lowPitchIndex + 1) % _lowPitch.Length;
        else
            _highPitchIndex = (_highPitchIndex + 1) % 4;

        Play();
    }
}
