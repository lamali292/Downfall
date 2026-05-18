using BaseLib.Utils;
using HermitMod.Utility;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Hermit.HermitCode.Cards.Uncommon;

/// <summary>
///     Deal 14 damage. Costs 1 less per Strike or Defend played this turn.
///     Cost reduction is simplified - uses AfterCardPlayed to track.
/// </summary>
public sealed class ShortFuse : HermitCardModel
{
    public ShortFuse() : base(3, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(18, 4);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        await CommonActions.CardAttack(this, play).WithHermitShortFuseHitFx()
            .Execute(ctx);
    }

    public override Task AfterCardEnteredCombat(CardModel card)
    {
        if (card != this || IsClone) return Task.CompletedTask;

        var amount = CombatManager.Instance.History.CardPlaysFinished.Count(e =>
            (e.CardPlay.Card.Tags.Contains(CardTag.Strike) || e.CardPlay.Card.Tags.Contains(CardTag.Defend)) &&
            e.CardPlay.Card.Owner == Owner && e.HappenedThisTurn(CombatState));
        ReduceCostBy(amount);
        return Task.CompletedTask;
    }

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner || (!cardPlay.Card.Tags.Contains(CardTag.Strike) &&
                                             !cardPlay.Card.Tags.Contains(CardTag.Defend)))
            return Task.CompletedTask;

        ReduceCostBy(1);
        return Task.CompletedTask;
    }

    private void ReduceCostBy(int amount)
    {
        EnergyCost.AddThisTurn(-amount);
    }
}

/* transform_cards.py changes:
 *   namespace → Hermit.HermitCode.Cards.Uncommon
 *   usings updated
 *   CanonicalVars removed → With* calls in constructor
 *   AdditionalHoverTips/ExtraHoverTips removed (covered by WithPower)
 *   OnUpgrade removed (all logic migrated to constructor)
 *   constructor: WithDamage(18, 4)
 *   DamageCmd.Attack chain → CommonActions.CardAttack
 */