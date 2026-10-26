using System.Runtime.CompilerServices;
using Downfall.DownfallCode.Config;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Screens.CardLibrary;

namespace Downfall.DownfallCode.Patches;

/// <summary>
/// Pairs the card library's "Multiplayer Cards" tickbox with a "Singleplayer Cards" one. Vanilla's box can only hide
/// multiplayer-only cards; with two boxes each kind of card is shown exactly when its own box is ticked, so unticking
/// Singleplayer leaves just the multiplayer-only cards (and unticking Multiplayer is the vanilla behavior).
/// </summary>
[HarmonyPatch]
public static class SingleplayerCardsFilterPatch
{
    private const string TickboxScenePath = "res://scenes/screens/card_library/card_library_tickbox.tscn";

    private static readonly ConditionalWeakTable<NCardLibrary, NTickbox> Tickboxes = new();

    [HarmonyPatch(typeof(NCardLibrary), nameof(NCardLibrary._Ready))]
    [HarmonyPostfix]
    private static void AddTickbox(NCardLibrary __instance)
    {
        if (!DownfallConfig.EnableCompendiumTools) return;

        try
        {
            var multiplayer = __instance.GetNode<NLibraryStatTickbox>("%MultiplayerCards");
            var container = multiplayer.GetParent<Control>();

            var tickbox = GD.Load<PackedScene>(TickboxScenePath).Instantiate<NLibraryStatTickbox>();
            tickbox.Name = "DownfallSingleplayerCards";
            container.AddChild(tickbox);
            container.MoveChild(tickbox, multiplayer.GetIndex());
            tickbox.SetLabel(new LocString("gameplay_ui", "DOWNFALL-LIBRARY_SINGLEPLAYER_CARDS").GetRawText());
            tickbox.IsTicked = true;
            tickbox.Connect(NTickbox.SignalName.Toggled, Callable.From<NTickbox>(_ => __instance.UpdateFilter()));

            // Keep controller navigation flowing top to bottom through the new row.
            if (__instance.GetNodeOrNull<Control>("%AlphabetSorter") is { } alphabet)
            {
                alphabet.FocusNeighborBottom = tickbox.GetPath();
                tickbox.FocusNeighborTop = alphabet.GetPath();
            }

            tickbox.FocusNeighborBottom = multiplayer.GetPath();
            multiplayer.FocusNeighborTop = tickbox.GetPath();

            Tickboxes.Add(__instance, tickbox);
        }
        catch (Exception e)
        {
            DownfallMainFile.Logger.Error($"Could not add the singleplayer cards tickbox to the card library: {e}");
        }
    }

    // Opening the library resets every vanilla filter and then calls UpdateFilter, so reset ours first.
    [HarmonyPatch(typeof(NCardLibrary), nameof(NCardLibrary.OnSubmenuOpened))]
    [HarmonyPrefix]
    private static void ResetTickbox(NCardLibrary __instance)
    {
        if (Tickboxes.TryGetValue(__instance, out var tickbox))
            tickbox.IsTicked = true;
    }

    // The library hands its finished filter to the grid, which is the one place to add to it without copying
    // UpdateFilter's body.
    [HarmonyPatch(typeof(NCardLibraryGrid), nameof(NCardLibraryGrid.FilterCards),
        [typeof(Func<CardModel, bool>), typeof(List<SortingOrders>)])]
    [HarmonyPrefix]
    private static void HideSingleplayerCards(NCardLibraryGrid __instance, ref Func<CardModel, bool> filter)
    {
        var library = FindLibrary(__instance);
        if (library == null || !Tickboxes.TryGetValue(library, out var tickbox) || tickbox.IsTicked)
            return;

        var vanilla = filter;
        filter = card => vanilla(card) && card.MultiplayerConstraint == CardMultiplayerConstraint.MultiplayerOnly;
    }

    private static NCardLibrary? FindLibrary(Node node)
    {
        for (var parent = node.GetParent(); parent != null; parent = parent.GetParent())
            if (parent is NCardLibrary library)
                return library;
        return null;
    }
}
