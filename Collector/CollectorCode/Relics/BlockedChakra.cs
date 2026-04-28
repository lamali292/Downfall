using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Events;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Collector.CollectorCode.Relics;

[Pool(typeof(CollectorRelicPool))]
public class BlockedChakra : CollectorRelicModel, IPreventCollectedDraw
{

    public BlockedChakra()
    {
        WithEnergy(1);
    }
    
    public override RelicRarity Rarity => RelicRarity.Ancient;
    
    public bool PreventCollectedDraw(Player player)
    {
        if (player != Owner) return false;
        return player.Creature.CombatState is { RoundNumber: <= 4 };
    }

    public override async Task AfterSideTurnStart(CombatSide side, ICombatState combatState)
    {
        if (side != Owner.Creature.Side)
            return;
        Flash();
        await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);
    }
}