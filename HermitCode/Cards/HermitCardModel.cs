using BaseLib.Utils;
using Downfall.DownfallCode.Abstract;
using HarmonyLib;
using Hermit.HermitCode.Core;
using Hermit.HermitCode.CustomEnums;
using Hermit.HermitCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
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
        TargetType targetType) : base(cost, type, rarity, targetType)
    {
        WithTips(e => e is not HermitCardModel card ? [] :
            card is IHasDeadOnEffect ? [HoverTipFactory.FromKeyword(HermitKeywords.DeadOn)] : Enumerable.Empty<IHoverTip>());
    }

    public bool IsDeadOn => HermitCmd.IsDeadOnInCurrentHandState(this) || (PatchDeadOnCapture.LastPlayed == this && PatchDeadOnCapture.LastWasDeadOn);
 
    protected override bool ShouldGlowGoldInternal => this is IHasDeadOnEffect && IsDeadOn;


    protected virtual Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay, bool isDeadOn)
            => PlayEffect(ctx, cardPlay);
    protected virtual async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await Task.CompletedTask;
    }

    protected sealed override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var isDeadOn = PatchDeadOnCapture.LastWasDeadOn; 
        if (CombatState == null) return;
        if (Keywords.Contains(HermitKeywords.Concentrate))
            await CommonActions.ApplySelf<ConcentrationPower>(ctx, this, 1);
        await PlayEffect(ctx, cardPlay, isDeadOn);
        if (this is IHasDeadOnEffect && isDeadOn)
        {
            await HermitCmd.TriggerDeadOnEffect(ctx, this, cardPlay);
        }

    }
}


[HarmonyPatch(typeof(CardModel), nameof(CardModel.OnPlayWrapper))]
static class PatchDeadOnCapture
{
    internal static bool LastWasDeadOn;
    internal static CardModel? LastPlayed;
    static void Prefix(CardModel __instance)
    {
        LastPlayed = __instance;
        LastWasDeadOn = HermitCmd.IsDeadOnInCurrentHandState(__instance);
    }
}