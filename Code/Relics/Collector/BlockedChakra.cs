using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Events;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.Relics;

namespace Downfall.Code.Relics.Collector;

[Pool(typeof(CollectorRelicPool))]
public class BlockedChakra : CollectorRelicModel, IPreventCollectedDraw
{

    public override RelicRarity Rarity => RelicRarity.Ancient;
    
    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side != Owner.Creature.Side)
            return;
        Flash();
        await PlayerCmd.GainEnergy(1, Owner);
    }

    public bool PreventCollectedDraw(Player player)
    {
        if (player != Owner) return false;
        return player.Creature.CombatState is { RoundNumber: <= 4 };
    }
}