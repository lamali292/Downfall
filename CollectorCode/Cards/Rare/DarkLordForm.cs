using BaseLib.Utils;
using Collector.CollectorCode.Cards.Basic;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Collector.CollectorCode.Cards.Rare;

[Pool(typeof(CollectorCardPool))]
public class DarkLordForm : CollectorCardModel
{
    public DarkLordForm() : base(4, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithTip(new TooltipSource(card => card.IsUpgraded
            ? HoverTipFactory.FromPower<DarkLordFormPlusPower>()
            : HoverTipFactory
                .FromPower<DarkLordFormPower>()));
        WithTip(typeof(YouAreMine));
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (IsUpgraded)
            await CommonActions.ApplySelf<DarkLordFormPlusPower>(ctx, this, 1);
        else
            await CommonActions.ApplySelf<DarkLordFormPower>(ctx, this, 1);
    }
}