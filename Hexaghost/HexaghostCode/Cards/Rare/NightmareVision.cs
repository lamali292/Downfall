using BaseLib.Utils;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Cards.Rare;

[Pool(typeof(HexaghostCardPool))]
public class NightmareVision : HexaghostCardModel
{
    public NightmareVision() : base(1, CardType.Power, CardRarity.Rare, TargetType.None)
    {
        WithPower<NightmareVisionPower>(4, 1);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<NightmareVisionPower>(ctx, this);
    }
}