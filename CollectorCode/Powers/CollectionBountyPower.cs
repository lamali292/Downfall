using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Collector.CollectorCode.Powers;

public class CollectionBountyPower : CollectorPowerModel
{

    public CollectionBountyPower() : base(PowerType.Debuff)
    {
        WithReserveTip();
    }
    
    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature,
        bool wasRemovalPrevented, float deathAnimLength)
    {
        if (creature != Owner) return;
        var players = creature.CombatState?.Players ?? Applier?.CombatState?.Players;
        if (players == null) return;
        foreach (var combatStatePlayer in players)
        {
            await CollectorCmd.GainReserve(combatStatePlayer, Amount);
        }
    }
}