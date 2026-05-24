using BaseLib.Utils;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Cards.Rare;

[Pool(typeof(HexaghostCardPool))]
public class VolcanoVisage : HexaghostCardModel
{
    public VolcanoVisage() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithPower<VolcanoVisagePower>(5, 2);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<VolcanoVisagePower>(ctx, this);
    }
}