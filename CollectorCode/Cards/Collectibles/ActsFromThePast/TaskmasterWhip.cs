using BaseLib.Utils;
using Collector.CollectorCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Collector.CollectorCode.Cards.Collectibles.ActsFromThePast;

public class TaskmasterWhip : ActsFromThePastCard
{
    public TaskmasterWhip() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, "SLAVERS_ELITE")
    {
        WithDamage(10, 2);
        WithPower<BlisterPower>(4, 1);
        WithKeyword(CardKeyword.Exhaust);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await CommonActions.Apply<BlisterPower>(ctx, this, cardPlay);
    }
}