using Downfall.Code.Abstract;
using Downfall.Code.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Powers.Champ;

public class UltimateStancePower() : ChampPowerModel(PowerType.Buff, PowerStackType.Single)
{
    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power != this || amount <= 0 || Owner.Player == null || LocalContext.NetId == null) return;
        var ctx = new HookPlayerChoiceContext(
            Owner.Player,
            LocalContext.NetId.Value,
            GameActionType.Combat);
        await ChampCmd.EnterUltimateStance(ctx, Owner.Player);
    }


    public override async Task AfterTurnEnd(PlayerChoiceContext ctx, CombatSide side)
    {
        if (side == Owner.Side || Owner.Player == null) return;
        await ChampCmd.ClearStance(ctx, Owner.Player);
        await PowerCmd.Remove(this);
    }
}