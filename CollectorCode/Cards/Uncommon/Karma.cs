using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Powers;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Collector.CollectorCode.Cards.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class Karma : CollectorCardModel
{
    public Karma() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<KarmaPower>(3, 1);
        WithVar("Debuffs", 2);
        WithTip(StaticHoverTip.Block);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<KarmaPower>(ctx, this);
    }
}