using BaseLib.Utils;
using Collector.CollectorCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Collector.CollectorCode.Cards.Collectibles.ActsFromThePast;

public class GremlinAxe : ActsFromThePastCard
{
    public GremlinAxe() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self, "GREMLIN_NOB_ELITE")
    {
        WithCostUpgradeBy(-1);
        WithPower<GremlinAxePower>(1, false);
        WithTip<VigorPower>();
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<GremlinAxePower>(ctx, this);
    }
}