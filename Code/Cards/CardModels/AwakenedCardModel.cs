using BaseLib.Abstracts;
using BaseLib.Extensions;
using Downfall.Code.Extensions;
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
    : ConstructedCardModel(cost, type, rarity, targetType)
{
    public bool HasChanted { get; set; } = false;
    public sealed override string PortraitPath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath<AwakenedCharacter>();

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
            if (this is IChantable chantable)
            {
                return WasLastCardPlayedPower || chantable.HasChanted;
            }
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
        {
            chantable.HasChanted = true;
            await chantable.OnChant(ctx, cardPlay);
            if (cardPlay.Card.CombatState == null) return;
            var listeners = cardPlay.Card.CombatState.IterateHookListeners().OfType<IOnChant>();
            foreach (var listener in listeners)
            {
                await listener.OnCardChanted(this, ctx, cardPlay);
            }
        }
    }
    
}