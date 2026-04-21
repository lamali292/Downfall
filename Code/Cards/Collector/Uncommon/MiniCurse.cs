using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Cards.Collector.Uncommon;

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
        await MyCommonActions.Apply<WeakPower>(this, cardPlay);
        await MyCommonActions.Apply<VulnerablePower>(this, cardPlay);
    }
}