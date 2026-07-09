using Downfall.DownfallCode.Interfaces;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Events.Custom;

namespace Downfall.DownfallCode.Patches;

[HarmonyPatch(typeof(NFakeMerchant), "StartCharacterAnimation")]
public static class FakeMerchantAnimationPatch
{
    [HarmonyPrefix]
    static bool Prefix(NCreatureVisuals visuals)
    {
        if (visuals is not IAnimatedVisuals animatedVisuals) return true;
        animatedVisuals.OnAnimationTrigger("Idle");
        return false;
    }
}