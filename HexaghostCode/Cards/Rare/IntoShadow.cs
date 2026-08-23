using BaseLib.Utils;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.CustomEnums;
using Hexaghost.HexaghostCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Cards.Rare;

[Pool(typeof(HexaghostCardPool))]
public class IntoShadow : HexaghostCardModel
{
    public IntoShadow() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        this.WithPower<IntoShadowPower>(1, false);
        WithTip(CardKeyword.Exhaust);
        WithTip(HexaghostKeyword.Retract);
        WithCostUpgradeBy(-1);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<IntoShadowPower>(ctx, this);
    }
}