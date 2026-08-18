using BaseLib.Utils;
using Gremlins.GremlinsCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Gremlins.GremlinsCode.Cards.Common;

[Pool(typeof(GremlinsCardPool))]
public class Pinprick : GremlinsCardModel
{
    public Pinprick() : base(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithKeyword(CardKeyword.Exhaust);
        WithDamage(1);
        WithCards(1);
    }


    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await CommonActions.Draw(this, ctx);
    }

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card != this || !IsUpgraded || this.IsEcho) return;
        var copy = card.CreateEcho();
        await CardPileCmd.Add(copy, PileType.Hand);
    }
}