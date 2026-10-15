using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.CustomEnums;
using SlimeBoss.SlimeBossCode.Powers;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Cards.Uncommon;

[Pool(typeof(SlimeBossCardPool))]
public class SplitScrap : SlimeBossCardModel
{
    public SplitScrap() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithTip(SlimeBossTip.Split);
        WithSlimeTip<ScrapSlime>();
        WithPower<PotencyPower>(0, 2, true);
        WithTags(SlimeBossTag.Slime);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var slime = await SlimeBossCmd.Split<ScrapSlime>(ctx, Owner);
        if (slime == null) return;
        await CommonActions.Apply<PotencyPower>(ctx, slime, this);
    }
}
