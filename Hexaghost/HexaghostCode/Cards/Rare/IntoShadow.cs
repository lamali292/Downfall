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
    public IntoShadow() : base(3, CardType.Power, CardRarity.Rare, TargetType.None)
    {
        WithPower<IntoShadowPower>(1);
        WithTip(CardKeyword.Exhaust);
        WithTip(HexaghostKeyword.Retract);
        WithCostUpgradeBy(-1);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<IntoShadowPower>(ctx, this);
    }
}