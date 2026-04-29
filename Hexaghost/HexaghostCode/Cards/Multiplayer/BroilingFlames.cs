using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Cards.Multiplayer;

[Pool(typeof(HexaghostCardPool))]
public class BroilingFlames : HexaghostCardModel
{
    
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;
    
    public BroilingFlames() : base(1, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithPower<BroilingFlamesPower>(5, 2);
        WithKeywords(CardKeyword.Exhaust);
    }
    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await MyCommonActions.Apply<BroilingFlamesPower>(ctx, this, cardPlay);
    }


}