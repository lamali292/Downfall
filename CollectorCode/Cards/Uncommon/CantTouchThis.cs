using BaseLib.Abstracts;
using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Powers;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Collector.CollectorCode.Cards.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class CantTouchThis : CollectorCardModel
{
    public CantTouchThis() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<DexterityPower>(1);
        WithPower<CantTouchThisPower>(1, 1, false);
        WithTip<MiasmaPower>();
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<DexterityPower>(ctx, this);
        await CommonActions.ApplySelf<CantTouchThisPower>(ctx, this);
    }
}