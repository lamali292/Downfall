using Downfall.DownfallCode.Utils.UI;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Downfall.DownfallCode.Patches;

[HarmonyPatch(typeof(NCombatPilesContainer))]
internal class CombatPilesContainerPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(NCombatPilesContainer._Ready))]
    private static void AddRegisteredPiles(NCombatPilesContainer __instance)
    {
        foreach (var type in CombatPileButtonRegistry.Types)
        {
            var scenePath = CombatPileButtonRegistry.ReadMetadata(type);
            var scene = ResourceLoader.Load<PackedScene>(scenePath);
            if (scene == null) continue;

            var button = (NCustomCombatCardPile)scene.Instantiate();
            __instance.AddChildSafely(button);
        }
    }

    // Phase 2: initialize them alongside the built-in piles (like _drawPile.Initialize(player))
    [HarmonyPostfix]
    [HarmonyPatch(nameof(NCombatPilesContainer.Initialize))]
    private static void InitializeRegisteredPiles(NCombatPilesContainer __instance, Player player)
    {
        foreach (var btn in __instance.GetChildren().OfType<NCustomCombatCardPile>())
            btn.Initialize(player);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(NCombatPilesContainer.AnimIn))]
    private static void AnimInAll(NCombatPilesContainer __instance)
    {
        foreach (var btn in __instance.GetChildren().OfType<NCustomCombatCardPile>())
            if (btn.Visible)
                btn.AnimIn();
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(NCombatPilesContainer.AnimOut))]
    private static void AnimOutAll(NCombatPilesContainer __instance)
    {
        foreach (var btn in __instance.GetChildren().OfType<NCustomCombatCardPile>())
            btn.PlayAnimOut();
    }
}