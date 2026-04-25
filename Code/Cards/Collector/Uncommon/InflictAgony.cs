using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Cards.Collector.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class InflictAgony : CollectorCardModel
{
    public InflictAgony() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(12, 6);
        WithPower<VulnerablePower>(2);
        WithPower<WeakPower>(2);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        if (!cardPlay.Target.IsAfflicted())
        {
            await CommonActions.Apply<WeakPower>(ctx, cardPlay.Target, this);
            await CommonActions.Apply<VulnerablePower>(ctx, cardPlay.Target, this);
        }
    }
}