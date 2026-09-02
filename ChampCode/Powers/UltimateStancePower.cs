using Champ.ChampCode.Core;
using Champ.ChampCode.Events;
using Champ.ChampCode.Stance;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Champ.ChampCode.Powers;

public class UltimateStancePower() : ChampPowerModel(PowerType.Buff, PowerStackType.Single), IOnChampStanceChange
{
    public Task OnChampStanceChange(PlayerChoiceContext ctx, Player player, ChampStanceModel oldStance,
        ChampStanceModel newStance)
    {
        if (Owner.Player != player || newStance is ChampUltimateStance or ChampNoStance) return Task.CompletedTask;
        return EnterUltimateStance(ctx, player);
    }

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext ctx, PowerModel power, decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        if (power != this || amount <= 0 || Owner.Player == null || LocalContext.NetId == null) return;
        await EnterUltimateStance(ctx, Owner.Player);
    }

    private static async Task EnterUltimateStance(PlayerChoiceContext ctx, Player player)
    {
        await ChampModel.SetStance<ChampUltimateStance>(ctx, player);
    }


    public override async Task AfterSideTurnEnd(PlayerChoiceContext ctx, CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side == Owner.Side || Owner.Player == null) return;
        await ChampCmd.ClearStance(ctx, Owner.Player);
        await PowerCmd.Remove(this);
    }
}