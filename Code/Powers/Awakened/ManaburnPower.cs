using Downfall.Code.Abstract;
using Downfall.Code.Interfaces;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.Code.Powers.Awakened;

public class ManaburnPower : AwakenedPowerModel, IOnDrained
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;


    public async Task OnDrained(Player player, DrainedPower drainedPower, int amount)
    {
        if (Applier != drainedPower.Owner || LocalContext.NetId == null) return;

        var ctx = new HookPlayerChoiceContext(
            player,
            LocalContext.NetId.Value,
            GameActionType.Combat);
        await CreatureCmd.Damage(ctx,
            Owner, Amount * amount,
            ValueProp.Move | ValueProp.Unblockable | ValueProp.Unpowered, drainedPower.Owner);
    }
}