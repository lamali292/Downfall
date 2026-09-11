using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Events;
using Downfall.DownfallCode.Compatibility;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Collector.CollectorCode.Powers;

public class FeelMyPainPower : CollectorPowerModel
{
    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var card = cardPlay.Card;
        if (card.Owner.Creature != Owner || !card.VisualCardPool.IsColorless) return;
        
        var bCtx = new BlockingPlayerChoiceContext();
        Flash();
        var currentEnemies = CombatState.Enemies.ToList();
        foreach (var enemy in currentEnemies)
            if (enemy is { IsHittable: true, IsAlive: true })
                await CreatureCmd.Damage(bCtx,
                    enemy,
                    Amount,
                    DamageProps.nonCardHpLoss,
                    Owner);
  
    }
}