using BaseLib.Extensions;
using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Powers.Collector;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Cards.Collector.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class BlackBindings : CollectorCardModel
{
    public BlackBindings() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithPower<WeakPower>(2);
        WithPower<CollectorDoomPower>(2, 2);
    }

    private static decimal DamageCalc(Creature? creature)
    {
        return creature?.Powers.Count(e => e.Type == PowerType.Debuff) ?? 0;
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        await CommonActions.Apply<WeakPower>(ctx,cardPlay.Target, this);
        var amount = DamageCalc(cardPlay.Target) * DynamicVars.Power<CollectorDoomPower>().BaseValue;
        await CommonActions.Apply<CollectorDoomPower>(ctx,cardPlay.Target, this, amount);
    }
}