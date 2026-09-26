using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Powers;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Cards.Uncommon;

// CSV row 28 names this "Taunting Slime" but its own effect text says "Split into a Leeching Slime" -
// treated as a naming/text swap error in the source spreadsheet; the NAME column is authoritative here.
[Pool(typeof(SlimeBossCardPool))]
public class SplitTaunting : SlimeBossCardModel
{
    public SplitTaunting() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithCostUpgradeBy(-1);
        WithSlimeTip<TauntingSlime>();
        WithPower<PotencyPower>(0, 2, true);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var slime = await SlimeBossCmd.Split<TauntingSlime>(ctx, Owner);
        if (slime == null) return;
        await CommonActions.Apply<PotencyPower>(ctx, slime, this);
    }
}
