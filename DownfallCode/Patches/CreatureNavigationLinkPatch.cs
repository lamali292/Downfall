using Downfall.DownfallCode.Utils.UI;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Downfall.DownfallCode.Patches;

/// <summary>
/// NCreature.UpdateNavigation() only re-links Hitbox.FocusNeighborTop for Defect (whose
/// OrbManager isn't null) after NCombatRoom.UpdateCreatureNavigation() resets it to a
/// self-loop on every turn start/end and targeting-session end. This gives every other
/// character's DownfallControllerNav.LinkAbove the same auto-repair, so custom widgets
/// (Ghostflame Wheel, stance icons, etc.) stay reachable across a full combat instead of
/// only until the next navigation refresh silently clobbers them.
/// </summary>
[HarmonyPatch(typeof(NCreature), nameof(NCreature.UpdateNavigation))]
public static class CreatureNavigationLinkPatch
{
    private static void Postfix(NCreature __instance)
    {
        DownfallControllerNav.ReapplyAnchorLink(__instance.Hitbox);
    }
}
