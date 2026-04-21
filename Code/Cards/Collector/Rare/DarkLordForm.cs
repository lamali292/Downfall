using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Cards.Collector.Basic;
using Downfall.Code.Powers.Collector;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Downfall.Code.Cards.Collector.Rare;

[Pool(typeof(CollectorCardPool))]
public class DarkLordForm : CollectorCardModel
{
    public DarkLordForm() : base(4, CardType.Power, CardRarity.Rare, TargetType.None)
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
            await CommonActions.ApplySelf<DarkLordFormPlusPower>(this, 1);
        else
            await CommonActions.ApplySelf<DarkLordFormPower>(this, 1);
    }
}