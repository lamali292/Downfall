using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Piles;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Collector.Rare;

[Pool(typeof(CollectorCardPool))]
public class HoardersStrike : CollectorCardModel
{
    public HoardersStrike() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(16, 4);
        WithTags(CardTag.Strike);
    }
    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        var cards = CollectorPile.Collected.GetPile(Owner).Cards;
        if (cards.Count == 0) return;
        var card = cards[0];
        await CardCmd.AutoPlay(ctx, card, cardPlay.Target);
    }
    
}