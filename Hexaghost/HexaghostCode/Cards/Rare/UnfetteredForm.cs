using BaseLib.Utils;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Hexaghost.HexaghostCode.Cards.Rare;

[Pool(typeof(HexaghostCardPool))]
public class UnfetteredForm : HexaghostCardModel
{
    public UnfetteredForm() : base(3, CardType.Power, CardRarity.Rare, TargetType.None)
    {
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

        HexaghostVisualsBridge.RefreshCurrentIntent(Owner);
    }
}