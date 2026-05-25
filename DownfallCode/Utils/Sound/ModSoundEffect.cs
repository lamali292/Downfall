using BaseLib.Audio;
using MegaCrit.Sts2.Core.Random;

namespace Downfall.DownfallCode.Utils.Sound;

public class ModSoundEffect
{
    private readonly ModSoundEntry[] _entries;
    private readonly float _globalPitchVariation;
    private readonly float _globalVolumeAdd;
    private readonly float _totalWeight;

    public ModSoundEffect(params ModSoundEntry[] entries)
        : this(0f, 0f, entries)
    {
    }

    public ModSoundEffect(float globalPitchVariation = 0f, float globalVolumeAdd = 0f,
        params ModSoundEntry[] entries)
    {
        _entries = entries;
        _globalPitchVariation = globalPitchVariation;
        _globalVolumeAdd = globalVolumeAdd;
        _totalWeight = entries.Sum(e => e.Weight);
    }

    public void Play()
    {
        PlayOn(e =>
        {
            MyModAudio.PlaySound(
                e.Sound,
                volumeAdd:_globalVolumeAdd + e.VolumeAdd,
                pitchVariation: _globalPitchVariation + e.PitchVariation,
                basePitch: e.BasePitch);
            
        });
    }

    public void PlayInRun()
    {
        PlayOn(e =>
        {
            MyModAudio.PlaySoundInRun(
                e.Sound,
                volumeAdd: _globalVolumeAdd + e.VolumeAdd,
                pitchVariation: _globalPitchVariation + e.PitchVariation,
                basePitch: e.BasePitch);
        });
    }

    private void PlayOn(Action<ModSoundEntry> play)
    {
        play(PickRandom());
    }

    private ModSoundEntry PickRandom()
    {
        var roll = (float)(Rng.Chaotic.NextDouble() * _totalWeight);
        var cumulative = 0f;
        foreach (var entry in _entries)
        {
            cumulative += entry.Weight;
            if (roll <= cumulative) return entry;
        }

        return _entries[^1];
    }
}