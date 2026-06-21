using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Cards.Rare;

[Pool(typeof(HexaghostCardPool))]
public class HereAndNow : HexaghostCardModel
{
    public HereAndNow() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithCostUpgradeBy(-1);
        WithPower<MoreEnergyPower>(1);
        WithPower<HereAndNowPower>(1);
    }

    protected override Artist Artist => Artist.Get<CartesianCanvas>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<MoreEnergyPower>(ctx, this);
        await CommonActions.ApplySelf<HereAndNowPower>(ctx, this);
    }
}