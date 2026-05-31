using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.CustomEnums;
using Champ.ChampCode.Powers;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Champ.ChampCode.Cards.Rare;

[Pool(typeof(ChampCardPool))]
public class DancingMaster : ChampCardModel
{
    public DancingMaster() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithTip(ChampTip.Finisher);
        WithEnergy(1);
        this.WithPower<DancingMasterPower>(1, false);
        WithCostUpgradeBy(-1);
    }

    protected override Artist Artist => Artist.Get<Opal>();


    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<DancingMasterPower>(ctx, this);
    }
}