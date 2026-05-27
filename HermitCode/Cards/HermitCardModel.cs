using BaseLib.Utils;
using Downfall.DownfallCode.Abstract;
using HarmonyLib;
using Hermit.HermitCode.Core;
using Hermit.HermitCode.CustomEnums;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace Hermit.HermitCode.Cards;

[Pool(typeof(HermitCardPool))]
public abstract class HermitCardModel
    : DownfallCardModel<Core.Hermit>
{
    protected HermitCardModel(
        int cost,
        CardType type,
        CardRarity rarity,
        TargetType targetType,
        bool showInCardLibrary = true,
        bool autoAdd = true) : base(cost, type, rarity, targetType, showInCardLibrary, autoAdd)
    {
        WithTips(e => e is IHasDeadOnEffect ? [HoverTipFactory.FromKeyword(HermitKeywords.DeadOn)] : []);
    }

    protected override bool ShouldGlowGoldInternal => this is IHasDeadOnEffect { IsDeadOn: true };
}

[HarmonyPatch(typeof(CardModel), nameof(CardModel.OnPlayWrapper))]
internal static class PatchDeadOnCapture
{
    internal static bool LastWasDeadOn;
    internal static CardModel? LastPlayed;

    private static void Prefix(CardModel __instance)
    {
        LastPlayed = __instance;
        LastWasDeadOn = HermitCmd.IsDeadOnInCurrentHandState(__instance);
    }
}