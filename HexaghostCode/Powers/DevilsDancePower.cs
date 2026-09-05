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

public class DevilsDancePower : HexaghostPowerModel, IWheelMoved
{
    private int UsesLeft { get; set; }

    public override int DisplayAmount => UsesLeft;

    public async Task AfterWheelAdvance(PlayerChoiceContext ctx, Player player, AbstractModel? source,
        GhostflameModel ghostflame,
        int ghostflameIndex, bool silent)
    {
        if (player.Creature != Owner) return;
        if (silent) return;
        if (UsesLeft  <= 0) return;
        await CardPileCmd.Draw(ctx, player);
        UsesLeft--;
        InvokeDisplayAmountChanged();
    }

    public async Task AfterWheelRetract(PlayerChoiceContext ctx, Player player, AbstractModel? source,
        GhostflameModel ghostflame,
        int ghostflameIndex, bool silent)
    {
        if (player.Creature != Owner) return;
        if (silent) return;
        if (UsesLeft  <= 0) return;
        await CardPileCmd.Draw(ctx, player);
        UsesLeft--;
        InvokeDisplayAmountChanged();
    }

    public override Task BeforeSideTurnEndEarly(PlayerChoiceContext choiceContext, CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner)) return Task.CompletedTask;
        UsesLeft = Amount;
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }
}