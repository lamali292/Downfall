using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Powers;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Cards.Uncommon;

[Pool(typeof(SlimeBossCardPool))]
public class SplitCultist : SlimeBossCardModel
{
    public SplitCultist() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithSlimeTip<CultistSlime>();
        WithPower<PotencyPower>(0, 2, true);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var slime = await SlimeBossCmd.Split<CultistSlime>(ctx, Owner);
        if (slime == null) return;
        await CommonActions.Apply<PotencyPower>(ctx, slime, this);
    }
}
