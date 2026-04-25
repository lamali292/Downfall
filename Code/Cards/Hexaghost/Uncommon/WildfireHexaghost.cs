using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Powers.Hexaghost;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Hexaghost.Uncommon;

[Pool(typeof(HexaghostCardPool))]
public class WildfireHexaghost : HexaghostCardModel
{
    public WildfireHexaghost() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.None)
    {
        WithPower<WildfirePower>(5, 7);
    }
    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<WildfirePower>(ctx, this);
    }
}