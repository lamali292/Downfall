using BaseLib.Utils;
using Collector.CollectorCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Collector.CollectorCode.Cards.Collectibles.Ruina2;

public class Recharge : Ruina2Card
{
    public Recharge() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self, "HELPERS_ELITE")
    {
        WithCostUpgradeBy(-1);
        WithPower<RechargePower>(1, false);
        WithReserveTip();
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<RechargePower>(ctx, this);
    }
}