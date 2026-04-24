using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Cards.Hexaghost.Common;

[Pool(typeof(HexaghostCardPool))]
public class PowerFromBeyond : HexaghostCardModel
{
    public PowerFromBeyond() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithAfterlife();
        WithPower<VigorPower>(3, 1);
        WithPower<EnergyNextTurnPower>(2, 3);
    }
    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await AfterlifeEffect(ctx, cardPlay);
        await CommonActions.ApplySelf<EnergyNextTurnPower>(ctx, this);
    }


    protected override async Task AfterlifeEffect(PlayerChoiceContext ctx, CardPlay? cardPlay = null)
    {
        await CommonActions.ApplySelf<VigorPower>(ctx, this);
    }
}