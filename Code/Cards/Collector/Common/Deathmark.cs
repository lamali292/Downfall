using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Powers.Collector;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Collector.Common;

[Pool(typeof(CollectorCardPool))]
public class Deathmark : CollectorCardModel
{
    public Deathmark() : base(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(7, 2);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        var attack = await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        var damage = 2 * attack.Results.Sum(e => e.UnblockedDamage);
        await PowerCmd.Apply<DeathmarkedPower>(ctx, cardPlay.Target, damage, Owner.Creature, this);
    }
}