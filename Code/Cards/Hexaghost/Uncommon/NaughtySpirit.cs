using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Core.Hexaghost;
using Downfall.Code.Powers.Hexaghost;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Downfall.Code.Cards.Hexaghost.Uncommon;

[Pool(typeof(HexaghostCardPool))]
public class NaughtySpirit : HexaghostCardModel
{
    public NaughtySpirit() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithPower<SoulBurnPower>(3, 2);
    }
    
    
    
    public override (PileType, CardPilePosition) ModifyCardPlayResultPileTypeAndPosition(
        CardModel card, bool isAutoPlay,
        ResourceInfo resources, PileType pileType, CardPilePosition position)
    {
        if (this != card || !HexaghostCmd.IsIgnited(card.Owner)) return (pileType, position); 
        
        return (PileType.Hand, position);
    }
    
    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.ResultPile != PileType.Hand || this != cardPlay.Card) return;
        await HexaghostCmd.Retract(ctx, Owner);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await MyCommonActions.Apply<SoulBurnPower>(ctx, this, cardPlay);
        
    }
}