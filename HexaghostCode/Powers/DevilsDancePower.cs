using BaseLib.Abstracts;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Events;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Hexaghost.HexaghostCode.Powers;

public class DevilsDancePower : HexaghostPowerModel, IWheelMoved, IHasSecondAmount
{
    private int UsesThisTurn { get; set; }

    public string GetSecondAmount()
    {
        return $"{UsesThisTurn}";
    }

    public async Task AfterWheelAdvance(PlayerChoiceContext ctx, Player player, AbstractModel? source,
        GhostflameModel ghostflame,
        int ghostflameIndex, bool silent)
    {
        if (silent) return;
        if (UsesThisTurn <= Amount) await CardPileCmd.Draw(ctx, player);
        UsesThisTurn++;
        if (UsesThisTurn <= Amount) InvokeDisplayAmountChanged();
    }

    public async Task AfterWheelRetract(PlayerChoiceContext ctx, Player player, AbstractModel? source,
        GhostflameModel ghostflame,
        int ghostflameIndex, bool silent)
    {
        if (silent) return;
        if (UsesThisTurn <= Amount) await CardPileCmd.Draw(ctx, player);
        UsesThisTurn++;
        if (UsesThisTurn <= Amount) InvokeDisplayAmountChanged();
    }

    public override Task BeforeSideTurnEndEarly(PlayerChoiceContext choiceContext, CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side != Owner.Side) return Task.CompletedTask;
        UsesThisTurn = 0;
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }
}