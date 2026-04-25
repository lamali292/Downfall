using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Powers.Hexaghost;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Hexaghost.Uncommon;


[Pool(typeof(HexaghostCardPool))]
public class GhostflameBarrier : HexaghostCardModel
{
    public GhostflameBarrier() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithBlock(12, 4);
        WithTip(typeof(SoulBurnPower));
        WithPower<GhostflameBarrierPower>(5, 7);
    }
    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
      await CommonActions.CardBlock(this, cardPlay);
      await CommonActions.ApplySelf<GhostflameBarrierPower>(ctx, this);
    }

}