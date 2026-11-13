using BaseLib.Utils;
using SlimeBoss.SlimeBossCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.CustomEnums;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Cards.Uncommon;

[Pool(typeof(SlimeBossCardPool))]
public class SplitMuscle : SlimeBossCardModel
{
    public SplitMuscle() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithTip(SlimeBossTip.Split);
        WithPower<PotencyPower>(0, 2, true);
        WithSlimeTip<MuscleSlime>();
        WithTags(SlimeBossTag.Slime);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var slime = await SlimeBossCmd.Split<MuscleSlime>(ctx, Owner);
        if (slime == null) return;
        await CommonActions.Apply<PotencyPower>(ctx, slime, this);
    }
}
