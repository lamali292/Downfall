using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Rare;

/// <summary>
///     Put 2 random Attacks from discard into hand. They cost 1 less this turn. Exhaust.
///     Upgrade: 3 attacks.
/// </summary>
public sealed class Gambit : HermitCardModel
{
    public Gambit() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithCards(2, 1);
        WithKeyword(CardKeyword.Exhaust);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        // Shuffle and take up to count
        for (var i = 0; i < DynamicVars.Cards.BaseValue; i++)
        {
            var discardedAttacks = PileType.Discard.GetPile(Owner).Cards.Where(c => c.Type == CardType.Attack);
            var combatCardSelection = Owner.RunState.Rng.CombatCardSelection;
            var card = combatCardSelection.NextItem(discardedAttacks);
            if (card == null) break;

            // Move from discard to hand and reduce cost by 1 this turn
            await CardPileCmd.Add(card, PileType.Hand);
            card.EnergyCost.AddThisTurnOrUntilPlayed(-1, true);
        }
    }
}

/* transform_cards.py changes:
 *   namespace → Hermit.HermitCode.Cards.Rare
 *   usings updated
 *   CanonicalVars removed → With* calls in constructor
 *   CanonicalKeywords removed → WithKeyword(...) in constructor
 *   OnUpgrade removed (all logic migrated to constructor)
 *   constructor: WithCards(2, 1), WithKeyword(CardKeyword.Exhaust)
 */