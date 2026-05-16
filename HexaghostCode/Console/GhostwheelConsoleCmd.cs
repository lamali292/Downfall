using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.DevConsole;
using MegaCrit.Sts2.Core.DevConsole.ConsoleCommands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Runs;

namespace Hexaghost.HexaghostCode.Console;

public class GhostwheelIgniteCmd : AbstractConsoleCmd
{
    public override string CmdName => "ignite";
    public override string Args => "[index:int]";
    public override string Description => "Ignite the current ghostflame, or a specific index if provided.";
    public override bool IsNetworked => true;

    public override CmdResult Process(Player? issuingPlayer, string[] args)
    {
        if (!RunManager.Instance.IsInProgress)
            return new CmdResult(false, "No run in progress.");
        if (issuingPlayer == null)
            return new CmdResult(false, "No player context.");

        var ctx = new BlockingPlayerChoiceContext();
        var wheel = HexaghostCmd.GetWheel(issuingPlayer);

        if (args.Length == 0)
        {
            var index = HexaghostCmd.GetCurrentIndex(issuingPlayer);
            TaskHelper.RunSafely(HexaghostCmd.Ignite(ctx, issuingPlayer));
            return new CmdResult(true, $"Ignited flame at index {index} ({wheel[index].GetType().Name}).");
        }

        if (!int.TryParse(args[0], out var i) || i < 0 || i >= wheel.Length)
            return new CmdResult(false, $"Invalid index. Valid range: 0-{wheel.Length - 1}.");

        TaskHelper.RunSafely(HexaghostCmd.IgniteAt(ctx, issuingPlayer, i));
        return new CmdResult(true, $"Ignited flame at index {i} ({wheel[i].GetType().Name}).");
    }

    public override CompletionResult GetArgumentCompletions(Player? player, string[] args)
    {
        if (args.Length != 1 || player == null || !RunManager.Instance.IsInProgress)
            return new CompletionResult { Type = CompletionType.Argument, ArgumentContext = CmdName };
        var wheel = HexaghostCmd.GetWheel(player);
        var options = Enumerable.Range(0, wheel.Length)
            .Select(i => $"{i} ({wheel[i].GetType().Name})")
            .ToList();
        return CompleteArgument(options, [], args[0]);
    }
}

public class GhostwheelAdvanceCmd : AbstractConsoleCmd
{
    public override string CmdName => "advance";
    public override string Args => "[steps:int]";
    public override string Description => "Advance the ghostwheel by 1 step, or N steps if provided.";
    public override bool IsNetworked => true;

    public override CmdResult Process(Player? issuingPlayer, string[] args)
    {
        if (!RunManager.Instance.IsInProgress)
            return new CmdResult(false, "No run in progress.");
        if (issuingPlayer == null)
            return new CmdResult(false, "No player context.");

        var steps = 1;
        if (args.Length > 0 && !int.TryParse(args[0], out steps))
            return new CmdResult(false, $"Invalid steps value '{args[0]}'.");

        var ctx = new BlockingPlayerChoiceContext();
        TaskHelper.RunSafely(AdvanceMultiple(ctx, issuingPlayer, steps));

        var index = HexaghostCmd.GetCurrentIndex(issuingPlayer);
        var flame = HexaghostCmd.GetCurrentFlame(issuingPlayer);
        return new CmdResult(true, $"Advanced {steps} step(s). Now at index {index} ({flame.GetType().Name}).");
    }

    private static async Task AdvanceMultiple(PlayerChoiceContext ctx, Player player, int steps)
    {
        for (var i = 0; i < steps; i++)
            await HexaghostCmd.Advance(ctx, player, null);
    }
}

public class GhostwheelRetractCmd : AbstractConsoleCmd
{
    public override string CmdName => "retract";
    public override string Args => "[steps:int]";
    public override string Description => "Retract the ghostwheel by 1 step, or N steps if provided.";
    public override bool IsNetworked => true;

    public override CmdResult Process(Player? issuingPlayer, string[] args)
    {
        if (!RunManager.Instance.IsInProgress)
            return new CmdResult(false, "No run in progress.");
        if (issuingPlayer == null)
            return new CmdResult(false, "No player context.");

        var steps = 1;
        if (args.Length > 0 && !int.TryParse(args[0], out steps))
            return new CmdResult(false, $"Invalid steps value '{args[0]}'.");

        var ctx = new BlockingPlayerChoiceContext();
        TaskHelper.RunSafely(RetractMultiple(ctx, issuingPlayer, steps));

        var index = HexaghostCmd.GetCurrentIndex(issuingPlayer);
        var flame = HexaghostCmd.GetCurrentFlame(issuingPlayer);
        return new CmdResult(true, $"Retracted {steps} step(s). Now at index {index} ({flame.GetType().Name}).");
    }

    private static async Task RetractMultiple(PlayerChoiceContext ctx, Player player, int steps)
    {
        for (var i = 0; i < steps; i++)
            await HexaghostCmd.Retract(ctx, player, null);
    }
}