using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Collector.CollectorCode.Cards.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class FeelMyPain : CollectorCardModel
{
    public FeelMyPain() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.None)
    {
        WithPower<FeelMyPainPower>(4, 1);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<FeelMyPainPower>(ctx, this);
    }
}