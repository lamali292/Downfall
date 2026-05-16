using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Collector.CollectorCode.Cards.Basic;

[Pool(typeof(CollectorCardPool))]
public class FuelTheFire : CollectorCardModel
{
    public FuelTheFire() : base(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
        WithPower<ReserveNextTurnPower>(2);
        WithPyre();
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<ReserveNextTurnPower>(ctx, this);
        if (IsUpgraded) await CommonActions.ApplySelf<DrawCardsNextTurnPower>(ctx, this, 1);
    }
}