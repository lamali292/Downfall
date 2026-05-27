using BaseLib.Utils;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Extensions;
using Hexaghost.HexaghostCode.Interfaces;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Hexaghost.HexaghostCode.Cards.Common;

[Pool(typeof(HexaghostCardPool))]
public class GhostLash : HexaghostCardModel, IHasAfterlifeEffect
{
    public GhostLash() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        this.WithAfterlife();
        WithCalculatedDamage(8, 3, Calc, ValueProp.Move, 2, 1);
    }

    public async Task AfterlifeEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    }

    private static decimal Calc(CardModel card, Creature? arg2)
    {
        return PileType.Hand.GetPile(card.Owner).Cards
            .Count(e => e != card && e.Keywords.Contains(CardKeyword.Ethereal));
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await AfterlifeEffect(ctx, cardPlay);
    }
}