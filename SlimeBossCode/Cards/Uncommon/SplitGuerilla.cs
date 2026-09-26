using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Cards.Uncommon;

[Pool(typeof(SlimeBossCardPool))]
public class SplitGuerilla : SlimeBossCardModel
{
    public SplitGuerilla() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithCostUpgradeBy(-1);
        WithSlimeTip<GuerillaSlime>();
    }

    protected override Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return SlimeBossCmd.Split<GuerillaSlime>(ctx, Owner);
    }
}
