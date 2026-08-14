using BaseLib.Audio;
using Downfall.DownfallCode.Audio;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Saves;

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
        FmodStudioServer.TryLogLoadedStudioBankEvents("res://Downfall/audio/Guardian.bank");
        float sfx = SaveManager.Instance.SettingsSave.VolumeSfx;
        if (FmodStudioGuidPathTable.TryGetStudioGuidForEventPath("event:/guardian/guardian_select", out var guid))
        {
            DownfallMainFile.Logger.Info($"[Downfall] guid={guid}");
            if (FmodStudioGuidInterop.TryNormalizeForAddon(guid, out var normalized))
            {
                DownfallMainFile.Logger.Info($"[Downfall] normalized={normalized}");
                var ok = FmodStudioGateway.TryCall(FmodStudioMethodNames.PlayOneShotUsingGuid, normalized, 1f);
                DownfallMainFile.Logger.Info($"[Downfall] play returned={ok}");
            }
            else DownfallMainFile.Logger.Warn("[Downfall] normalize FAILED");
        }
        else DownfallMainFile.Logger.Warn("[Downfall] path not in guid table");
        /*
        PlayOn(e =>
        {
            ModAudio.PlaySound(
                e.Sound,
                _globalVolumeAdd + e.VolumeAdd,
                pitchVariation: _globalPitchVariation + e.PitchVariation,
                basePitch: e.BasePitch);
        });*/
    }

    public void PlayInRun()
    {
        PlayOn(e =>
        {
            ModAudio.PlaySoundInRun(
                e.Sound,
                _globalVolumeAdd + e.VolumeAdd,
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