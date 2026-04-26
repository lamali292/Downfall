using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Powers.Hexaghost;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Hexaghost.Rare;

[Pool(typeof(HexaghostCardPool))]
public class VolcanoVisage : HexaghostCardModel
{
    public VolcanoVisage() : base(1, CardType.Power, CardRarity.Rare, TargetType.None)
    {
        WithPower<VolcanoVisagePower>(5, 2);
    }
    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<VolcanoVisagePower>(ctx, this);
    }


}