using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Powers;

namespace SlimeBoss.SlimeBossCode.Cards.Rare;

[Pool(typeof(SlimeBossCardPool))]
public class DouseInSlime : SlimeBossCardModel
{
    public DouseInSlime() : base(3, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithPower<GoopPower>(14);
        WithCostUpgradeBy(-1);
        WithPower<DouseInSlimePower>(1);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.Apply<GoopPower>(ctx, this, cardPlay);
        await CommonActions.Apply<DouseInSlimePower>(ctx, this, cardPlay);
    }
}