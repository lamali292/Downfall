using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.CustomEnums;

namespace SlimeBoss.SlimeBossCode.Cards.Common;

[Pool(typeof(SlimeBossCardPool))]
public class HeavyTackle : SlimeBossCardModel
{
    // How many Status plays this turn are already reflected in this card's cost, so syncing is idempotent
    // (a card can be drawn, discarded and redrawn within one turn).
    private int _discountedRound = -1;
    private int _discountedStatuses;

    public HeavyTackle() : base(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithTags(SlimeBossTag.Tackle);
        WithDamage(15, 5);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    }

    public override Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Card.Type == CardType.Status && cardPlay.Card.Owner == Owner && Pile?.Type == PileType.Hand)
            SyncStatusDiscount();
        return Task.CompletedTask;
    }

    public override Task AfterCardDrawn(PlayerChoiceContext ctx, CardModel card, bool fromHandDraw)
    {
        if (card == this) SyncStatusDiscount();
        return Task.CompletedTask;
    }

    private void SyncStatusDiscount()
    {
        var combatState = Owner.Creature.CombatState;
        if (combatState == null) return;

        var statusesThisTurn = CombatManager.Instance.History.Entries
            .OfType<CardPlayStartedEntry>()
            .Count(e => e.CardPlay.Card.Type == CardType.Status && e.CardPlay.Card.Owner == Owner
                                                                 && e.HappenedThisTurn(combatState));
        if (_discountedRound != combatState.RoundNumber)
        {
            _discountedRound = combatState.RoundNumber;
            _discountedStatuses = 0;
        }

        var delta = statusesThisTurn - _discountedStatuses;
        if (delta <= 0) return;
        EnergyCost.AddThisTurn(-delta);
        _discountedStatuses = statusesThisTurn;
    }
}
