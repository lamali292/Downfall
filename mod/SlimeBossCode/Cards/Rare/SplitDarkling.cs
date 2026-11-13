using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.CustomEnums;
using SlimeBoss.SlimeBossCode.Powers;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Cards.Rare;

[Pool(typeof(SlimeBossCardPool))]
public class SplitDarkling : SlimeBossCardModel
{
    public SplitDarkling() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithTip(SlimeBossTip.Split);
        WithSlimeTip<DarklingSlime>();
        WithPower<PotencyPower>(0, 2, true);
        WithTags(SlimeBossTag.Slime);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var slime = await SlimeBossCmd.Split<DarklingSlime>(ctx, Owner);
        if (slime == null) return;
        await CommonActions.Apply<PotencyPower>(ctx, slime, this);
    }
}
