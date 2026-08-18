using BaseLib.Utils;
using Gremlins.GremlinsCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Gremlins.GremlinsCode.Cards.Uncommon;

[Pool(typeof(GremlinsCardPool))]
public class ShowOfHands : GremlinsCardModel
{
    public ShowOfHands() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithCards(0, 1);
        WithCalculatedBlock(0, 2, Calc);
    }

    private static decimal Calc(CardModel card, Creature? arg2)
    {
        return card.Owner.Hand.Count(e => card != e);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.Draw(this, ctx);
        await CommonActions.CardBlock(this, cardPlay);
    }
}