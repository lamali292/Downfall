using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.CustomEnums;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Cards.Uncommon;

[Pool(typeof(SlimeBossCardPool))]
public class SplitLeeching : SlimeBossCardModel
{
    public SplitLeeching() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithTip(SlimeBossTip.Split);
        WithSlimeTip<LeechingSlime>();
        WithTags(SlimeBossTag.Slime);
        WithCostUpgradeBy(-1);
    }

    protected override Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return SlimeBossCmd.Split<LeechingSlime>(ctx, Owner);
    }
}
