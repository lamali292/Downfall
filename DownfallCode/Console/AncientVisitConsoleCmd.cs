using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Ancients;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.DevConsole;
using MegaCrit.Sts2.Core.DevConsole.ConsoleCommands;
using MegaCrit.Sts2.Core.Entities.Players;
using System;
using System.Linq;

public static class AncientDebug
{
    public static int? ForcedVisitIndex;
}

[HarmonyPatch(typeof(AncientDialogueSet), nameof(AncientDialogueSet.GetValidDialogues))]
static class ForceVisitIndexPatch
{
    static void Prefix(ref int charVisits, ref int totalVisits)
    {
        if (AncientDebug.ForcedVisitIndex is not { } v) return;
        charVisits = v;
        totalVisits = Math.Max(totalVisits, 1); // skip the totalVisits==0 first-ever short-circuit
        AncientDebug.ForcedVisitIndex = null;   // consume once: immune to _ExitTree race
    }
}

public class AncientVisitConsoleCmd : AbstractConsoleCmd
{
    public override string CmdName => "downfall-ancient";
    public override string Args => "<id:string> <index:int>";
    public override string Description => "Opens an ancient event forcing a specific visit/win index";
    public override bool IsNetworked => true;

    public override CmdResult Process(Player? issuingPlayer, string[] args)
    {
        if (args.Length < 2)
            return new CmdResult(false, "Usage: downfall-ancient <id> <index>");
        if (!int.TryParse(args[1], out var index) || index < 0)
            return new CmdResult(false, "index must be a non-negative integer.");

        var model = ModelDb.GetByIdOrNull<EventModel>(
            new ModelId(ModelDb.GetCategory(typeof(EventModel)), args[0].ToUpperInvariant()));
        if (model is not AncientEventModel && model is not MegaCrit.Sts2.Core.Models.Events.TheArchitect)
            return new CmdResult(false, "Invalid ancient ID.");

        AncientDebug.ForcedVisitIndex = index;  // set before EnterRoom; consumed by the next selection

        var room = new EventRoom(model);
        issuingPlayer?.RunState.AppendToMapPointHistory(MapPointType.Ancient, RoomType.Event, model.Id);
        return new CmdResult(RunManager.Instance.EnterRoom(room), true,
            $"Opened {args[0].ToUpperInvariant()} at index {index}");
    }

    public override CompletionResult GetArgumentCompletions(Player? player, string[] args)
    {
        if (args.Length <= 1)
            return CompleteArgument(
                ModelDb.AllAncients.Select((Func<AncientEventModel, string>)(a => a.Id.Entry)).ToList(),
                Array.Empty<string>(),
                args.FirstOrDefault() ?? "");

        return new CompletionResult { Type = CompletionType.Argument, ArgumentContext = CmdName };
    }
}