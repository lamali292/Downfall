using Guardian.GuardianCode.DynamicVars;
using Hexaghost.HexaghostCode.DynamicVars;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Hexaghost.HexaghostCode.Extensions;

public static class DynamicVarSetExtension
{
    private static T? GhostflameVarOrNull<T>(DynamicVarSet vard, string key) where T : GhostflameVar
    {
        return vard._vars.TryGetValue(key, out var v) ? v as T : null;
    }

    public static int GhostflameBlock(this DynamicVarSet vard)
        => GhostflameVarOrNull<GhostflameBlockVar>(vard, "Block")?.IntensityValue ?? 0;

    public static int GhostflameDamage(this DynamicVarSet vard)
        => GhostflameVarOrNull<GhostflameDamageVar>(vard, "Damage")?.IntensityValue ?? 0;

    public static int GhostflameSoulburn(this DynamicVarSet vard)
        => GhostflameVarOrNull<GhostflameSoulburnVar>(vard, "Soulburn")?.IntensityValue ?? 0;
}