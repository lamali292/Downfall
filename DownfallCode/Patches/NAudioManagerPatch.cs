using BaseLib.Audio;
using Downfall.DownfallCode.Utils.Sound;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;

namespace Downfall.DownfallCode.Patches;

[HarmonyPatch(typeof(SfxCmd), nameof(SfxCmd.Play), typeof(string), typeof(float))]
internal static class SfxOverridePatch
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

    private static bool Prefix(string sfx)
    {
        if (!sfx.StartsWith("res://")) return true; // let FMOD paths through normally

        if (Overrides.GetValueOrDefault(sfx) is { } effect)
        {
            effect.Play();
            return false;
        }

        ModAudio.PlaySoundGlobal(new ModSound(sfx));
        return false;
    }
}