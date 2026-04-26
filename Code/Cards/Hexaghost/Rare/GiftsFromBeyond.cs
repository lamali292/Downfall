using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Powers.Hexaghost;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Hexaghost.Rare;

[Pool(typeof(HexaghostCardPool))]
public class GiftsFromBeyond : HexaghostCardModel
{
    public GiftsFromBeyond() : base(2, CardType.Power, CardRarity.Rare, TargetType.None)
    {
        WithCostUpgradeBy(-1);
        WithPower<GiftsFromBeyondPower>(1);
    }
    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<GiftsFromBeyondPower>(ctx, this);
    }
}