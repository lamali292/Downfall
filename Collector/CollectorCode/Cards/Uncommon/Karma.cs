using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Powers;
using Downfall.DownfallCode.Powers.Downfall;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Collector.CollectorCode.Cards.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class Karma : CollectorCardModel
{
    public Karma() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.None)
    {
        WithPower<KarmaPower>(2, 1);
        WithPower<MetallicizePower>(2, 1);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<MetallicizePower>(ctx, this);
        await CommonActions.ApplySelf<KarmaPower>(ctx, this);
    }
}