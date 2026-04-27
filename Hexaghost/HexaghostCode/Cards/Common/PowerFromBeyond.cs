using BaseLib.Utils;
using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Hexaghost.HexaghostCode.Cards.Common;

[Pool(typeof(HexaghostCardPool))]
public class PowerFromBeyond : HexaghostCardModel
{
    public PowerFromBeyond() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithAfterlife();
        WithPower<VigorPower>(3, 1);
        WithEnergy(2, 1);
        WithPower<EnergyNextTurnPower>(2, 1);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await AfterlifeEffect(ctx, cardPlay);
        await CommonActions.ApplySelf<EnergyNextTurnPower>(ctx, this);
    }


    protected override async Task AfterlifeEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<VigorPower>(ctx, this);
    }
}