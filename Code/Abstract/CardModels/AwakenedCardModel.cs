using Downfall.Code.Abstract;
using Downfall.Code.Commands;
using Downfall.Code.Interfaces;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using AwakenedCharacter = Downfall.Code.Character.Awakened;

namespace Downfall.Code.Cards.CardModels;

public abstract class AwakenedCardModel(
    int cost,
    CardType type,
    CardRarity rarity,
    TargetType targetType)
    : DownfallCardModel<AwakenedCharacter>(cost, type, rarity, targetType)
{
    public bool HasChanted { get; set; } = false;

    private bool WasLastCardPlayedPower
    {
        get
        {
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
}