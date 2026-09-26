using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Cards.Uncommon;

[Pool(typeof(SlimeBossCardPool))]
public class SplitPsychic : SlimeBossCardModel
{
    public SplitPsychic() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithCostUpgradeBy(-1);
        WithSlimeTip<PsychicSlime>();
    }

    protected override Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return SlimeBossCmd.Split<PsychicSlime>(ctx, Owner);
    }
}
