using BaseLib.Audio;
using Downfall.DownfallCode.Utils.Sound;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Nodes.Audio;

namespace Downfall.DownfallCode.Patches;

public static class SfxOverrideRegistry
{
    private static readonly Dictionary<string, ModSoundEffect> Overrides = new();
    
    public static void Register(string path, ModSoundEffect effect)
    {
        Overrides[path] = effect;
    }

    public static bool TryHandleResPath(string path)
    {
        if (!path.StartsWith("res://")) return false;

        if (Overrides.GetValueOrDefault(path) is { } effect)
        {
            effect.Play();
            return true;
        }

        ModAudio.PlaySoundGlobal(new ModSound(path));
        return true;
    }
}

// catch the override at multiple places because some of the original functions get inlined by jit so some patches not work all the time
[HarmonyPatch(typeof(SfxCmd), nameof(SfxCmd.Play), typeof(string), typeof(float))]
internal static class SfxOverridePatch
{
    [HarmonyPrefix]
    public static bool Prefix(string sfx)
    {
        return !SfxOverrideRegistry.TryHandleResPath(sfx);
    }
}

[HarmonyPatch(typeof(NAudioManager), nameof(NAudioManager.PlayOneShot), typeof(string), typeof(float))]
internal static class PlayOneShotPatch
{
    [HarmonyPrefix]
    public static bool Prefix(string path, float volume)
    {
        return !SfxOverrideRegistry.TryHandleResPath(path);
    }
}

[HarmonyPatch(typeof(NAudioManager), nameof(NAudioManager.PlayOneShot), typeof(string), typeof(Dictionary<string, float>), typeof(float))]
internal static class PlayOneShotDictPatch
{
    [HarmonyPrefix]
    public static bool Prefix(string path, float volume)
    {
        return !SfxOverrideRegistry.TryHandleResPath(path);
    }
}