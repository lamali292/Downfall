using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Cards.Rare;

[Pool(typeof(SlimeBossCardPool))]
public class DarklingDuo : SlimeBossCardModel
{
    public DarklingDuo() : base(3, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithVar("Count", 2, 1);
        WithSlimeTip<DarklingSlime>();
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var count = DynamicVars["Count"].IntValue;
        for (var i = 0; i < count; i++) await SlimeBossCmd.SplitForced<DarklingSlime>(ctx, Owner);
    }
}
