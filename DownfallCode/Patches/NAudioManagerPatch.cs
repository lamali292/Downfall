using BaseLib.Audio;
using Downfall.DownfallCode.Utils.Sound;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using Godot;
using MegaCrit.Sts2.Core.Nodes.Audio;

namespace Downfall.DownfallCode.Patches;

public static class SfxOverrideRegistry
{
    private static readonly Dictionary<string, ModSoundEffect> Overrides = new();

    public static ModSoundEffect? GetOverride(string path)
    {
        return Overrides.GetValueOrDefault(path);
    }

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

[HarmonyPatch(typeof(SfxCmd), nameof(SfxCmd.Play), typeof(string), typeof(float))]
internal static class SfxOverridePatch
{
    [HarmonyPrefix]
    public static bool Prefix(string sfx)
    {
        GD.Print($"[SfxOverridePatch] Prefix hit: {sfx}");
        return !SfxOverrideRegistry.TryHandleResPath(sfx);
    }
}

[HarmonyPatch(typeof(NAudioManager), nameof(NAudioManager.PlayOneShot), typeof(string), typeof(float))]
internal static class PlayOneShotPatch
{
    [HarmonyPrefix]
    public static bool Prefix(string path, float volume)
    {
        GD.Print($"[PlayOneShotPatch] path: {path}");
        return !SfxOverrideRegistry.TryHandleResPath(path);
    }
}

[HarmonyPatch(typeof(NAudioManager), nameof(NAudioManager.PlayOneShot), typeof(string), typeof(Dictionary<string, float>), typeof(float))]
internal static class PlayOneShotDictPatch
{
    [HarmonyPrefix]
    public static bool Prefix(string path, float volume)
    {
        GD.Print($"[PlayOneShotDictPatch] path: {path}");
        return !SfxOverrideRegistry.TryHandleResPath(path);
    }
}