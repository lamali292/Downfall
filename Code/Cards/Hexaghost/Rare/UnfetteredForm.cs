using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Powers.Collector;
using Downfall.Code.Powers.Hexaghost;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Downfall.Code.Cards.Hexaghost.Rare;

[Pool(typeof(HexaghostCardPool))]
public class UnfetteredForm : HexaghostCardModel
{
    public UnfetteredForm() : base(2, CardType.Power, CardRarity.Rare, TargetType.None)
    {
        WithCostUpgradeBy(-1);
        WithTip(new TooltipSource(card => card.IsUpgraded
            ? HoverTipFactory.FromPower<UnfetteredFormPlusPower>()
            : HoverTipFactory.FromPower<UnfetteredFormPower>()));
    }
    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (IsUpgraded)
            await CommonActions.ApplySelf<UnfetteredFormPlusPower>(ctx, this, 1);
        else
            await CommonActions.ApplySelf<UnfetteredFormPower>(ctx, this, 1);
    }
}