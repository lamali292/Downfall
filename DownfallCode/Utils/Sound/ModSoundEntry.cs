using BaseLib.Audio;

namespace Downfall.DownfallCode.Utils.Sound;

public class ModSoundEntry(
    string path,
    float weight = 1f,
    float pitchVariation = 0f,
    float basePitch = 1f,
    float volumeAdd = 0f)
{
    public ModSound Sound { get; } = new(path);
    public float Weight { get; } = weight;
    public float PitchVariation { get; } = pitchVariation;
    public float BasePitch { get; } = basePitch;
    public float VolumeAdd { get; } = volumeAdd;
}