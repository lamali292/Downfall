using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Events;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Hexaghost.HexaghostCode.Powers;

public class DevilsDancePower : HexaghostPowerModel, IWheelMoved
{
    public override int DisplayAmount => Math.Max(0, Amount - HexaghostCmd.CountWheelMovesThisTurn(Owner));

    public async Task AfterWheelAdvance(PlayerChoiceContext ctx, Player player, AbstractModel? source,
        GhostflameModel ghostflame,
        int ghostflameIndex, bool endOfTurnAdvance)
    {
        if (player.Creature != Owner) return;
        if (endOfTurnAdvance) return;
        await DrawIfWithinLimit(ctx, player);
    }

    public async Task AfterWheelRetract(PlayerChoiceContext ctx, Player player, AbstractModel? source,
        GhostflameModel ghostflame,
        int ghostflameIndex)
    {
        if (player.Creature != Owner) return;
        await DrawIfWithinLimit(ctx, player);
    }

    // The display counts this turn's moves, so a new turn resets it without any state change.
    public override Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (participants.Contains(Owner)) InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }

    // The move that fired this hook is already in the combat history, so it counts toward the limit.
    private async Task DrawIfWithinLimit(PlayerChoiceContext ctx, Player player)
    {
        if (HexaghostCmd.CountWheelMovesThisTurn(player.Creature) > Amount) return;
        await CardPileCmd.Draw(ctx, player);
        InvokeDisplayAmountChanged();
    }
}