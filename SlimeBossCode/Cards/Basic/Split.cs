using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Extensions;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Cards.Basic;

[Pool(typeof(SlimeBossCardPool))]
public class Split : SlimeBossCardModel
{
    public Split() : base(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
        this.WithCommand(1);
        WithCostUpgradeBy(-1);
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await SlimeBossCmd.Split<GuerillaSlime>(ctx, Owner);
        await SlimeBossCmd.Command(ctx, this);
    }
}