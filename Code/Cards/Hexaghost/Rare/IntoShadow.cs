using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Keywords;
using Downfall.Code.Powers.Hexaghost;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Hexaghost.Rare;

[Pool(typeof(HexaghostCardPool))]
public class IntoShadow : HexaghostCardModel
{
    public IntoShadow() : base(3, CardType.Power, CardRarity.Rare, TargetType.None)
    {
        WithPower<IntoShadowPower>(1);
        WithTip(CardKeyword.Exhaust);
        WithTip(DownfallKeywords.Retract);
        WithCostUpgradeBy(-1);
    }
    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<IntoShadowPower>(ctx, this);
    }


}