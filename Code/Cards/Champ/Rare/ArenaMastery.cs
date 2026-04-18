using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Powers.Champ;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Champ.Rare;

[Pool(typeof(ChampCardPool))]
public class ArenaMastery : ChampCardModel
{
    public ArenaMastery() : base(1, CardType.Power, CardRarity.Rare, TargetType.None)
    {
        WithPower<ArenaMasteryBerserkerPower>(1);
        WithPower<ArenaMasteryDefensivePower>(3, 1);
    }
    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<ArenaMasteryBerserkerPower>(this);
        await CommonActions.ApplySelf<ArenaMasteryDefensivePower>(this);
    }
}