using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Cards.Uncommon;

[Pool(typeof(SlimeBossCardPool))]
public class SplitMuscle : SlimeBossCardModel
{
    public SplitMuscle() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithCostUpgradeBy(-1);
        WithSlimeTip<MuscleSlime>();
    }

    protected override Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return SlimeBossCmd.Split<MuscleSlime>(ctx, Owner);
    }
}
