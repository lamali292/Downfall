using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Powers;

namespace SlimeBoss.SlimeBossCode.Cards.Uncommon;

[Pool(typeof(SlimeBossCardPool))]
public class ProtectTheBoss : SlimeBossCardModel
{
    public ProtectTheBoss() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        this.WithPower<ProtectTheBossPower>(1, false);
        WithCostUpgradeBy(-1);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return CommonActions.ApplySelf<ProtectTheBossPower>(ctx, this);
    }
}