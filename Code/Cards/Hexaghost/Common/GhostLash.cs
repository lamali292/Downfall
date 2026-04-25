using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.Code.Cards.Hexaghost.Common;

[Pool(typeof(HexaghostCardPool))]
public class GhostLash : HexaghostCardModel
{
    public GhostLash() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithAfterlife();
        WithCalculatedDamage(8, 3, Calc, ValueProp.Move, 2, 1);
    }

    private static decimal Calc(CardModel card, Creature? arg2)
    {
        return PileType.Hand.GetPile(card.Owner).Cards
            .Count(e => e != card && e.Keywords.Contains(CardKeyword.Ethereal));
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await AfterlifeEffect(ctx, cardPlay);
    }

    protected override async Task AfterlifeEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    }
}