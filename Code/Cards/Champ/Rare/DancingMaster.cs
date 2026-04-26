using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Keywords;
using Downfall.Code.Powers.Champ;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Champ.Rare;

[Pool(typeof(ChampCardPool))]
public class DancingMaster : ChampCardModel
{
    public DancingMaster() : base(2, CardType.Power, CardRarity.Rare, TargetType.None)
    {
        WithTip(DownfallTip.Finisher);
        WithEnergyTip();
        WithPower<DancingMasterPower>(1);
        WithCostUpgradeBy(-1);
    }


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<DancingMasterPower>(ctx, this);
    }
}