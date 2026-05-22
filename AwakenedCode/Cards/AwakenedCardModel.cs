using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.CustomEnums;
using Awakened.AwakenedCode.Interfaces;
using Awakened.AwakenedCode.Powers;
using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using AwakenedCharacter = Awakened.AwakenedCode.Core.Awakened;

namespace Awakened.AwakenedCode.Cards;

public abstract class AwakenedCardModel : DownfallCardModel<AwakenedCharacter>
{
    protected AwakenedCardModel(
        int cost,
        CardType type,
        CardRarity rarity,
        TargetType targetType,
        bool showInCardLibrary = true,
        bool autoAdd = true)
        : base(cost, type, rarity, targetType, showInCardLibrary, autoAdd)
    {
        WithTips(card => card is IChantable chantable
            ? chantable.HasChanted
                ? [HoverTipFactory.Static(AwakenedTip.Chanted)]
                : [HoverTipFactory.Static(AwakenedTip.Chant)]
            : []);
    }


    public bool HasChanted { get; set; } = false;

    public bool WasLastCardPlayedPower
    {
        get
        {
            if (!CombatManager.Instance.IsInProgress) return false;
            var lastCardEntry = CombatManager.Instance.History.CardPlaysStarted
                .LastOrDefault(e =>
                    e.CardPlay.Card.Owner == Owner &&
                    e.CardPlay.Card != this);

            if (lastCardEntry == null) return false;

            return lastCardEntry.CardPlay.Card.Type == CardType.Power;
        }
    }

    protected override bool ShouldGlowGoldInternal
    {
        get
        {
            if (this is IChantable chantable) return WasLastCardPlayedPower || chantable.HasChanted;
            return false;
        }
    }

    protected virtual async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await Task.CompletedTask;
    }

    protected sealed override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await PlayEffect(ctx, cardPlay);
        if (this is IChantable chantable && (WasLastCardPlayedPower || chantable.HasChanted))
            await AwakenedCmd.Chant(ctx, this, cardPlay);
    }


    protected AwakenedCardModel WithConjure(Func<CardModel, bool>? a = null)
    {
        if (a == null)
            WithTip(AwakenedTip.Conjure);
        else
            WithTips(e => a.Invoke(e) ? [HoverTipFactory.Static(AwakenedTip.Conjure)] : []);

        WithTags(AwakenedTag.Conjure);
        return this;
    }

    protected AwakenedCardModel WithDrained(int baseVal, int upgrade = 0)
    {
        WithPower<DrainedPower>(baseVal, upgrade, false);
        WithEnergy(baseVal, upgrade);
        return this;
    }

    protected sealed override void AddExtraArgsToDescription(LocString description)
    {
        base.AddExtraArgsToDescription(description);
    }
}