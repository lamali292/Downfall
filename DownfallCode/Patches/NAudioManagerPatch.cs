using BaseLib.Audio;
using Downfall.DownfallCode.Utils.Sound;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Nodes.Audio;
using MegaCrit.Sts2.Core.TestSupport;

namespace Downfall.DownfallCode.Patches;

[HarmonyPatch(typeof(NAudioManager), nameof(NAudioManager.PlayOneShot), typeof(string),
    typeof(Dictionary<string, float>), typeof(float))]
internal class NAudioManagerPatch
{
    private static bool Prefix(string path)
    {
        if (TestMode.IsOn) return true;
        if (!path.StartsWith("res://")) return true;

        // Check for ModSoundEffect override first
        if (SfxOverridePatch.GetOverride(path) is { } effect)
        {
            effect.Play();
            return false;
        }

        // Plain res:// sound — play via ModAudio
        ModAudio.PlaySoundGlobal(new ModSound(path));
        return false;
    }
}

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
        if (Overrides.GetValueOrDefault(sfx) is not { } effect) return true;
        effect.Play();
        return false;
    }
}