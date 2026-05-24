using BaseLib.Utils;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Cards.Rare;

[Pool(typeof(HexaghostCardPool))]
public class EmpoweredFlame : HexaghostCardModel
{
    public EmpoweredFlame() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<IntensityPower>(2, 1);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<IntensityPower>(ctx, this);
    }
}