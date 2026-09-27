using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Powers;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Cards.Uncommon;

[Pool(typeof(SlimeBossCardPool))]
public class SpreadingSlime : SlimeBossCardModel
{
    public SpreadingSlime() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithCostUpgradeBy(-1);
        WithPower<SpreadingSlimePower>(1, 1, false);
        WithTip<PotencyPower>();
        WithSlimeTip<BruiserSlime>();
    }

    protected override Artist Artist => Artist.Get<Thelethargicweirdo>();

    protected override Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return CommonActions.ApplySelf<SpreadingSlimePower>(ctx, this);
    }
}
