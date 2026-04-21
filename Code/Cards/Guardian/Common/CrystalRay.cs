using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Cards.Guardian.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.Code.Cards.Guardian.Common;

[Pool(typeof(GuardianCardPool))]
public class CrystalRay : GuardianCardModel
{
    public CrystalRay() : base(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithCalculatedDamage(12, 2, Calc, ValueProp.Move, 4, 1);
    }

    private static decimal Calc(CardModel card, Creature? creature)
    {
        var gemsInCards = PileType.Deck.GetPile(card.Owner).Cards.OfType<GuardianCardModel>()
            .Aggregate(0m, (acc, g) => acc + g.Gems.Count);
        var gemCards = PileType.Deck.GetPile(card.Owner).Cards.OfType<IGemCard>().Count();
        return gemsInCards + gemCards;
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    }
}