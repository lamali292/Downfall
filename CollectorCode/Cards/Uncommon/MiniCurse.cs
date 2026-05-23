using BaseLib.Utils;
using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Collector.CollectorCode.Cards.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class MiniCurse : CollectorCardModel
{
    public MiniCurse() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithPyre();
        WithPower<WeakPower>(1);
        WithPower<VulnerablePower>(1);
    }

    public override TargetType TargetType => IsUpgraded ? TargetType.AllEnemies : TargetType.AnyEnemy;

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.Apply<WeakPower>(ctx, this, cardPlay);
        await CommonActions.Apply<VulnerablePower>(ctx, this, cardPlay);
    }
}