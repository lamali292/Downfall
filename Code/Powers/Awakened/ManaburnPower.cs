using Downfall.Code.Abstract;
using Downfall.Code.Events;
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


    public async Task OnDrained(Player player, int amount)
    {
        if (Applier != player.Creature || LocalContext.NetId == null) return;

        var ctx = new HookPlayerChoiceContext(
            player,
            LocalContext.NetId.Value,
            GameActionType.Combat);
        await CreatureCmd.Damage(ctx,
            Owner, Amount * amount,
            ValueProp.Move | ValueProp.Unblockable | ValueProp.Unpowered, player.Creature);
    }
}