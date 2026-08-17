using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Events;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using Snecko.SneckoCode.Ancients;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Patches;

public static class SneckoSpiritDialoguePatch
{
    [HarmonyPatch(typeof(NEventRoom), nameof(NEventRoom.RefreshEventState))]
    [HarmonyPrefix]
    private static bool RefreshPrefix(NEventRoom __instance, EventModel eventModel)
    {
        if (eventModel is not SneckoSpirit spirit)
            return true;

        if (__instance.Layout is not NAncientEventLayout layout)
            return true;                       // layout not ready yet -> let stock run

        var lines = spirit.CurrentTranscriptLines;
        if (lines.Count == 0)
            return true;                       // nothing to show yet -> stock path

        __instance.SetDescription(__instance.GetDescriptionOrFallback());
        layout.ClearDialogue();
        layout.SetDialogue(lines);
        __instance.SetOptions(eventModel);

        int last = lines.Count - 1;
        Callable.From(() =>
        {
            for (int i = 0; i <= last; i++)
                layout._dialogueContainer.GetChild<NAncientDialogueLine>(i)?.SetSpeakerIconVisible();
            layout.SetDialogueLineAndAnimate(last);
        }).CallDeferred();

        return false;
    }
}

public static class SneckoSpiritGate
{
    public static bool Done;
    public static void Reset() => Done = false;
}


[HarmonyPatch(typeof(RunManager), nameof(RunManager.EnterMapCoord))]
public static class SneckoSpiritEntryPatch
{
    private static bool Prefix(RunManager __instance, MapCoord coord, ref Task __result)
    {
        if (SneckoSpiritGate.Done) return true;

        var state = __instance.State;
        if (state is not { CurrentActIndex: 0 }) return true;
        if (!coord.Equals(state.Map.StartingMapPoint.coord)) return true;
        if (!state.Players.Any(p => p.Character is Core.Snecko)) return true;

        SneckoSpiritGate.Done = true;
        __result = EnterSethiru(__instance);
        return false;
    }

    private static async Task EnterSethiru(RunManager rm)
    {
        var sethiru = ModelDb.AncientEvent<SneckoSpirit>();
        await rm.EnterRoom(new EventRoom(sethiru));
    }
}

[HarmonyPatch(typeof(RunManager), nameof(RunManager.CleanUp))]
public static class SneckoSpiritGateResetPatch
{
    private static void Postfix() => SneckoSpiritGate.Reset();
}

[HarmonyPatch(typeof(NEventRoom), "SetOptions")]
public static class SneckoSpiritAutoSkipPatch
{
    private static bool Prefix(EventModel eventModel)
    {
        if (eventModel is not SneckoSpirit) return true;
        if (!eventModel.IsFinished) return true;
        if (eventModel.Owner?.Character is Core.Snecko) return true;
        
        TaskHelper.RunSafely(NEventRoom.Proceed());
        return false;
    }
}