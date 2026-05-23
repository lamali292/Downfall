using Downfall.DownfallCode.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Uncommon;

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
        while (totalCost < threshold && Owner.GetHand().Count < CardPile.MaxCardsInHand)
        {
            var cards = (await CardPileCmd.Draw(ctx, 1, Owner)).ToList();
            if (cards.Count == 0)
                break;
            var card = cards.First();
            totalCost += card.EnergyCost.GetWithModifiers(CostModifiers.Local);
        }
    }
}