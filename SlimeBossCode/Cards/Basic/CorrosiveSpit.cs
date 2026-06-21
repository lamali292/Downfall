using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Powers;

namespace SlimeBoss.SlimeBossCode.Cards.Basic;

[Pool(typeof(SlimeBossCardPool))]
public class CorrosiveSpit : SlimeBossCardModel
{
    public CorrosiveSpit() : base(1, CardType.Skill, CardRarity.Basic, TargetType.AnyEnemy)
    {
        WithPower<GoopPower>(6);
        WithCostUpgradeBy(-1);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.Apply<GoopPower>(ctx, this, cardPlay);
    }
}