using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Piles;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Collector.CollectorCode.Cards.Rare;

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