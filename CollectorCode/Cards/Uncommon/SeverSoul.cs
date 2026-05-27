using BaseLib.Utils;
using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Cards.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class SeverSoul : CollectorCardModel
{
    public SeverSoul() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(16, 6);
        WithTip(CardKeyword.Exhaust);
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var cardsToExhaust = GetCards().ToList();

        foreach (var card in cardsToExhaust) await CardCmd.Exhaust(ctx, card);

        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    }


    private IEnumerable<CardModel> GetCards()
    {
        return Owner.GetHand(c => c.Type != CardType.Attack);
    }
}