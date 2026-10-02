using BaseLib.Utils;
using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Collector.CollectorCode.Cards.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class BidingBlast : CollectorCardModel
{
    public BidingBlast() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(9, 2);
        WithReserveTip();
    }

    
    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var reserve = CollectorEnergy.Instance;
        var usedReserve = reserve != null && reserve.WasSpentOn(this);
        var hits = 1;
        if (usedReserve) hits++;
        await CommonActions.CardAttack(this, cardPlay, hits).Execute(ctx);

    }
}