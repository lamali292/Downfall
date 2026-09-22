using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace Collector.CollectorCode.Powers;

public class FeelMyPainPower : CollectorPowerModel
{
    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var card = cardPlay.Card;
        if (card.Owner.Creature != Owner || !card.VisualCardPool.IsColorless) return;
        Flash();
        await CreatureCmd.Damage(ctx,
            CombatState.HittableEnemies,
            Amount,
            DamageProps.nonCardHpLoss,
            Owner);
  
    }
}