using HarmonyLib;
using MegaCrit.Sts2.Core.Models.Powers;
using Downfall.DownfallCode.Interfaces;
using MegaCrit.Sts2.Core.Models;

[HarmonyPatch(typeof(DexterityPower), nameof(DexterityPower.ModifyBlockAdditive))]
public static class IgnoreDexterityPatch
{
    public static bool Prefix(CardModel? cardSource, ref decimal __result)
    {
        if (cardSource is not IIgnoreDexterityCard { ShouldIgnoreDexterity: true }) return true;
        __result = 0M;
        return false;
    }
}