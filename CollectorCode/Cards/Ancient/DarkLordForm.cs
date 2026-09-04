using BaseLib.Utils;
using Collector.CollectorCode.Cards.Basic;
using Collector.CollectorCode.Cards.Rare;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Powers;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Collector.CollectorCode.Cards.Ancient;

[Pool(typeof(CollectorCardPool))]
public class DarkLordForm : CollectorCardModel
{
    public DarkLordForm() : base(5, CardType.Power, CardRarity.Ancient, TargetType.Self)
    {
        WithUpgradingCardTip<YouAreMine>();
        WithKeyword(CardKeyword.Retain);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (IsUpgraded)
            await CommonActions.ApplySelf<DarkLordFormPlusPower>(ctx, this, 1);
        else
            await CommonActions.ApplySelf<DarkLordFormPower>(ctx, this, 1);
    }
}