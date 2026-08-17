using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Events;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using Snecko.SneckoCode.Ancients;

namespace Snecko.SneckoCode.Patches;

public static class SneckoSpiritDialoguePatch
{
    [HarmonyPatch(typeof(NEventRoom), nameof(NEventRoom.RefreshEventState))]
    [HarmonyPrefix]
    private static bool RefreshPrefix(NEventRoom __instance, EventModel eventModel)
    {
        if (eventModel is not SneckoSpirit spirit)
            return true;

        __instance.SetDescription(__instance.GetDescriptionOrFallback());

        var layout = (NAncientEventLayout)__instance.Layout;
        layout.ClearDialogue();
        layout.SetDialogue(spirit.CurrentTranscriptLines);
        __instance.SetOptions(eventModel);

        int last = spirit.CurrentTranscriptLines.Count - 1;
        Callable.From(() =>
        {
            for (int i = 0; i <= last; i++)
                layout._dialogueContainer.GetChild<NAncientDialogueLine>(i)?.SetSpeakerIconVisible();
            layout.SetDialogueLineAndAnimate(last);
        }).CallDeferred();

        return false;
    }
}

[HarmonyPatch(typeof(NEventOptionButton), nameof(NEventOptionButton._Ready))]
public static class SneckoSpiritOptionIconPatch
{
    private static void Postfix(NEventOptionButton __instance)
    {
        if (__instance.Event is not SneckoSpirit spirit) return;
        if (!spirit.OptionCharacters.TryGetValue(__instance.Option, out var character)) return;

        // reuse the ancient button's relic-icon slot for the character portrait
        var icon = __instance.GetNode<TextureRect>("%RelicIcon");
        if (icon == null) return;

        icon.SetTexture(character.IconTexture);
        icon.GetNode<TextureRect>("%Outline")?.SetTexture(character.IconOutlineTexture);
        icon.Visible = true;
    }
}