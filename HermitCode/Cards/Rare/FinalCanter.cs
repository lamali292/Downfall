using BaseLib.Utils;
using Hermit.HermitCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Hermit.HermitCode.Cards.Rare;

/// <summary>
///     Deal 10 damage for each Curse in your hand. Retain. Exhaust.
///     Upgrade: 13 damage.
/// </summary>
public sealed class FinalCanter : HermitCardModel
{
    public FinalCanter() : base(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(10, 3);
        WithCalculatedVar("CalculatedHits", 0, CountCursesInHand);
        WithKeyword(CardKeyword.Retain);
        WithKeyword(CardKeyword.Exhaust);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);

        var hitCount = (int)((CalculatedVar)DynamicVars["CalculatedHits"]).Calculate(play.Target);
        if (hitCount <= 0)
            return;

        await CommonActions.CardAttack(this, play, hitCount)
            .WithHermitFireHitFx()
            .Execute(ctx);
    }

    private static decimal CountCursesInHand(CardModel card, Creature? _)
    {
        return PileType.Hand.GetPile(card.Owner).Cards.Count(c => c.Type == CardType.Curse);
    }
}

/* transform_cards.py changes:
 *   namespace → Hermit.HermitCode.Cards.Rare
 *   usings updated
 *   CanonicalKeywords removed → WithKeyword(...) in constructor
 *   OnUpgrade removed (all logic migrated to constructor)
 *   constructor: WithDamage(10, 3), WithKeyword(CardKeyword.Retain), WithKeyword(CardKeyword.Exhaust)
 *   DamageCmd.Attack chain → CommonActions.CardAttack
 */