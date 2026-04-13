using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Relics.Awakened;

[Pool(typeof(AwakenedRelicPool))]
public class RippedDoll : AwakenedRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Starter;
    /*
     * TODO: needs to call update methods
    public override int DisplayAmount
    {
        get
        {
            var combatState = Owner.Creature.CombatState;
            if (combatState == null) return 1;
            if (combatState.RoundNumber > 2) return -1;
            return combatState.RoundNumber;
        }
    }

    public override bool ShowCounter => CombatManager.Instance.IsInProgress && Status == RelicStatus.Normal;
    
    */
    
    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side != Owner.Creature.Side || combatState.RoundNumber > 2)
            return;
        Flash();
        await AwakenedCmd.Conjure(Owner, combatState);
    }


    
    public override RelicModel GetUpgradeReplacement() => ModelDb.Relic<ShreddedDoll>();
}