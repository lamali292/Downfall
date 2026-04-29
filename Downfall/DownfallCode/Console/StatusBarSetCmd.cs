using Downfall.DownfallCode.Vfx;
using MegaCrit.Sts2.Core.DevConsole;
using MegaCrit.Sts2.Core.DevConsole.ConsoleCommands;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Downfall.DownfallCode.Console;

public class StatusBarSetCmd : AbstractConsoleCmd
{
    public override string CmdName => "statusbar";
    public override string Args => "<current:int> <max:int>";
    public override string Description => "Set the status bar for the issuing player.";
    public override bool IsNetworked => false;

    public override CmdResult Process(Player? issuingPlayer, string[] args)
    {
        if (issuingPlayer == null)
            return new CmdResult(false, "No player context.");
        if (args.Length < 2)
            return new CmdResult(false, "Usage: statusbar <current> <max>");
        if (!int.TryParse(args[0], out var current))
            return new CmdResult(false, $"Invalid current value '{args[0]}'.");
        if (!int.TryParse(args[1], out var max))
            return new CmdResult(false, $"Invalid max value '{args[1]}'.");
        if (max < 0 || max > 5)
            return new CmdResult(false, "Max must be between 0 and 5.");
        if (current < 0 || current > max)
            return new CmdResult(false, $"Current must be between 0 and {max}.");

        StatusBarHelper.SetStatus(issuingPlayer, current, max, null);
        return new CmdResult(true, $"Status bar set to {current}/{max}.");
    }
}