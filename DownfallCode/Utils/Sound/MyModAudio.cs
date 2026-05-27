using BaseLib.Audio;
using Godot;

namespace Downfall.DownfallCode.Utils.Sound;

public static class MyModAudio
{
    public static AudioStreamPlayer? PlaySound(
        ModSound sound,
        float volumeAdd = 0.0f,
        float volumeMult = 1f,
        float pitchVariation = 0.0f,
        float basePitch = 1f,
        Node? targetNode = null)
    {
        return ModAudio.PlaySound(sound, volumeAdd, volumeMult, pitchVariation, basePitch, targetNode);
    }

    public static AudioStreamPlayer? PlaySoundGlobal(
        ModSound sound,
        float volumeAdd = 0.0f,
        float volumeMult = 1f,
        float pitchVariation = 0.0f,
        float basePitch = 1f)
    {
        return ModAudio.PlaySoundGlobal(sound, volumeAdd, volumeMult, pitchVariation, basePitch);
        ;
    }

    public static AudioStreamPlayer? PlaySoundInRun(
        ModSound sound,
        float volumeAdd = 0.0f,
        float volumeMult = 1f,
        float pitchVariation = 0.0f,
        float basePitch = 1f)
    {
        return ModAudio.PlaySoundInRun(sound, volumeAdd, volumeMult, pitchVariation, basePitch);
    }
}