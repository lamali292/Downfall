using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Downfall.Code.Cards.Collector.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class SeverSoul : CollectorCardModel
{
    public SeverSoul() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(16, 6);
        WithTip(CardKeyword.Exhaust);
    }
    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var cardsToExhaust = GetCards().ToList(); 

        foreach (var card in cardsToExhaust)
        {
            await CardCmd.Exhaust(ctx, card);
        }
    
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    }
    
    
    private IEnumerable<CardModel> GetCards()
    {
        return PileType.Hand.GetPile(Owner).Cards.Where(c => c.Type != CardType.Attack);
    }
}