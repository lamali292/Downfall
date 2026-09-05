using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Events;
using Downfall.DownfallCode.Compatibility;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Collector.CollectorCode.Powers;

public class FeelMyPainPower : CollectorPowerModel, IAfterCardPyred
{
    public FeelMyPainPower()
    {
        WithTip(CollectorKeyword.Pyre);
    }
    
    
    public async Task AfterCardPyred(PlayerChoiceContext ctx, CardModel card, CardModel pyred)
    {
        if (pyred.Owner.Creature != Owner) return;
        var creature = CombatState.RunState.Rng.CombatTargets.NextItem(CombatState.HittableEnemies);
        if (creature == null) return;
        await CompatibilityCreatureCmd.Damage(ctx, creature, Amount,
            DamageProps.nonCardHpLoss, Owner, null, null);
        Flash();
    }
}