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
    
    extension(DynamicVarSet vars)
    {
        public int GhostflameBlock => GhostflameVarOrNull<GhostflameBlockVar>(vars, "Block")?.IntensityValue ?? 0;

        public int GhostflameDamage
            => GhostflameVarOrNull<GhostflameDamageVar>(vars, "Damage")?.IntensityValue ?? 0;

        public int GhostflameSoulburn
            => GhostflameVarOrNull<GhostflameSoulburnVar>(vars, "Soulburn")?.IntensityValue ?? 0;
    }
 
}