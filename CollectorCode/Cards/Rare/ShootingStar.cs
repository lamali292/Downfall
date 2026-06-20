using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Collector.CollectorCode.Cards.Rare;

[Pool(typeof(CollectorCardPool))]
public class ShootingStar : CollectorCardModel
{
    public ShootingStar() : base(3, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithPower<ShootingStarPower>(1);
        WithCostUpgradeBy(-1);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<ShootingStarPower>(ctx, this);
    }
}