using Downfall.Code.Abstract;
using Downfall.Code.Core.Hexaghost;
using Downfall.Code.Events;
using Downfall.Code.Interfaces;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Powers.Hexaghost;

public class DevilsDancePower : HexaghostPowerModel, IWheelMoved, IHasSecondAmount
{
    private int UsesThisTurn { get; set; }

    public string GetSecondAmount()
    {
        return $"{UsesThisTurn}";
    }

    public async Task AfterWheelAdvance(PlayerChoiceContext ctx, Player player, GhostflameModel ghostflame,
        int ghostflameIndex, bool silent)
    {
        if (silent) return;
        if (UsesThisTurn <= Amount) await CardPileCmd.Draw(ctx, player);
        UsesThisTurn++;
        if (UsesThisTurn <= Amount) InvokeDisplayAmountChanged();
    }

    public async Task AfterWheelRetract(PlayerChoiceContext ctx, Player player, GhostflameModel ghostflame,
        int ghostflameIndex, bool silent)
    {
        if (silent) return;
        if (UsesThisTurn <= Amount) await CardPileCmd.Draw(ctx, player);
        UsesThisTurn++;
        if (UsesThisTurn <= Amount) InvokeDisplayAmountChanged();
    }


    public override Task AfterTurnEndLate(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != Owner.Side) return Task.CompletedTask;
        UsesThisTurn = 0;
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }
}