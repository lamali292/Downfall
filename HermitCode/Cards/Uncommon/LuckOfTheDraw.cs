using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Uncommon;

/// <summary>
///     Draw cards until total cost drawn is 3 or more.
///     Upgrade: Until total cost is 4 or more.
/// </summary>
public sealed class LuckOfTheDraw : HermitCardModel
{
    public LuckOfTheDraw() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithVar("Threshold", 3, 1);
    }


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        var threshold = DynamicVars["Threshold"].IntValue;
        var totalCost = 0;
        while (totalCost < threshold && PileType.Hand.GetPile(Owner).Cards.Count < 10)
        {
            var cards = (await CardPileCmd.Draw(ctx, 1, Owner)).ToList();
            if (cards.Count == 0)
                break;

            var card = cards[0];
            totalCost += card.EnergyCost.GetWithModifiers(CostModifiers.Local);
        }
    }
}

/* transform_cards.py changes:
 *   namespace → Hermit.HermitCode.Cards.Uncommon
 *   usings updated
 *   OnUpgrade removed (all logic migrated to constructor)
 */