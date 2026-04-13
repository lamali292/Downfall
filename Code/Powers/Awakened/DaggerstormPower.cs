using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.Code.Powers.Awakened;

public class DaggerstormPower : AwakenedPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    
    
    
    public override async Task AfterCardGeneratedForCombat(CardModel card, bool addedByPlayer)
    {
        if (card.Owner.Creature != Owner||LocalContext.NetId==null) return;
        var ctx = new HookPlayerChoiceContext(
            card.Owner,
            LocalContext.NetId.Value,
            GameActionType.Combat);
        var enemy = card.Owner.RunState.Rng.CombatTargets.NextItem(CombatState.Enemies);
        if (enemy == null) return;
        await CreatureCmd.Damage(ctx, enemy, Amount, ValueProp.Unpowered, Owner, null);
    }
}