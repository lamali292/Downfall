using Downfall.DownfallCode.Abstract;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Patches;

/// <summary>
/// Applies the upgrades declared through <see cref="ConstructedCardModel" /> (keyword add/remove, cost upgrade) when a
/// card is upgraded. BaseLib does the same for its own ConstructedCardModel, but only matches that exact type.
/// </summary>
[HarmonyPatch(typeof(CardModel), nameof(CardModel.UpgradeInternal))]
public static class ConstructedUpgradePatch
{
    public static void Postfix(CardModel __instance)
    {
        if (__instance is ConstructedCardModel constructed) constructed.ConstructedUpgrade();
    }
}
