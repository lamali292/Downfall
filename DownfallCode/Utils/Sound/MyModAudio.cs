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
        var a  = ModAudio.PlaySound(sound, volumeAdd, volumeMult, pitchVariation, basePitch, targetNode);
        if (a != null) a.VolumeDb = volumeAdd;
        return a;
    }

    public static AudioStreamPlayer? PlaySoundGlobal(
        ModSound sound,
        float volumeAdd = 0.0f,
        float volumeMult = 1f,
        float pitchVariation = 0.0f,
        float basePitch = 1f)
    {
        var a =  ModAudio.PlaySoundGlobal(sound, volumeAdd, volumeMult, pitchVariation, basePitch);
        if (a != null) a.VolumeDb = volumeAdd;
        return a;
    }
    
    public static AudioStreamPlayer? PlaySoundInRun(
        ModSound sound,
        float volumeAdd = 0.0f,
        float volumeMult = 1f,
        float pitchVariation = 0.0f,
        float basePitch = 1f)
    {
        var a = ModAudio.PlaySoundInRun(sound, volumeAdd, volumeMult, pitchVariation, basePitch);
        if (a != null) a.VolumeDb = volumeAdd;
        return a;
    }

}