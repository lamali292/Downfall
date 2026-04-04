using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Powers.Champ;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Champ.Uncommon;

[Pool(typeof(ChampCardPool))]
public class DoubleStyle : ChampCardModel
{
    public DoubleStyle() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.None)
    {
        WithPower<DefensiveStylePower>(1);
        WithPower<BerserkerStylePower>(1);
    }
    
    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await MyCommonActions.ApplySelf<DefensiveStylePower>(this);
        await MyCommonActions.ApplySelf<BerserkerStylePower>(this);
    }


    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}