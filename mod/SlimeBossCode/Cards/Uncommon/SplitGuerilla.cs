using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.CustomEnums;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Cards.Uncommon;

[Pool(typeof(SlimeBossCardPool))]
public class SplitGuerilla : SlimeBossCardModel
{
    public SplitGuerilla() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithTip(SlimeBossTip.Split);
        WithCostUpgradeBy(-1);
        WithSlimeTip<GuerillaSlime>();
        WithTags(SlimeBossTag.Slime);
    }

    protected override Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return SlimeBossCmd.Split<GuerillaSlime>(ctx, Owner);
    }
}
