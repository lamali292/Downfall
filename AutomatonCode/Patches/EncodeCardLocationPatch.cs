using System.Reflection;
using Automaton.AutomatonCode.Core;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Patches;

/// <summary>
///     Vanilla powers like Rebound decide whether to redirect a played card (e.g. to the draw
///     pile) purely by checking the card's about-to-happen result location: if it isn't already
///     something other than Discard (Exhaust cards already resolve to <see cref="PileType.Exhaust" />
///     here), they redirect it and consume a charge. Encode isn't a vanilla pile concept, so an
///     Encodable card still resolves to Discard at this point even though
///     <see cref="AutomatonCombatModel.AfterCardPlayed" /> is about to forcibly move it
///     into the Encode pile a moment later - Rebound "successfully" redirects a card that was never
///     actually going to be discarded, and wastes a charge for nothing.
///     Fix: report the location as <see cref="PileType.None" /> for cards that will be encoded, the
///     same way Power/Dupe cards already opt out of the discard/exhaust pile resolution, so vanilla
///     "am I about to be discarded" checks (Rebound included) see the same "not Discard" answer they
///     already give Exhaust cards.
///     New game version only. Compiled against the OLD assembly, so CardLocation must never appear
///     in typed code - accessed via reflection/Traverse only. Must only be added when CardLocation
///     exists at runtime (see AutomatonMainFile).
/// </summary>
[HarmonyPatch]
public static class EncodeCardResultLocationNewPatch
{
    private static readonly System.Type? CardLocationType =
        AccessTools.TypeByName("MegaCrit.Sts2.Core.Entities.Cards.CardLocation");

    private static MethodBase? TargetMethod()
    {
        return AccessTools.Method(typeof(CardModel), "GetResultLocationForCardPlay");
    }

    // __result as object: Harmony boxes the CardLocation struct for us (see ModifyCardPlayResultLocationPatch).
    private static void Postfix(CardModel __instance, ref object __result)
    {
        if (!AutomatonCmd.WillAutoEncode(__instance)) return;

        var tr = Traverse.Create(__result);
        if (tr.Field("pileType").GetValue<PileType>() != PileType.Discard) return;

        var player = tr.Field("player").GetValue<Player>();
        var position = tr.Field("position").GetValue<CardPilePosition>();
        __result = System.Activator.CreateInstance(CardLocationType!, player, PileType.None, position)!;
    }
}

/// <summary>
///     Old game version only: same fix as <see cref="EncodeCardResultLocationNewPatch" />, but the
///     old engine reports the result location as a plain (PileType, CardPilePosition) tuple via
///     <c>Hook.ModifyCardPlayResultPileTypeAndPosition</c>, computed from a starting pileType/position
///     pair with no CardModel-side hook point to postfix beforehand - so this patches the vanilla
///     per-listener loop's *input* instead, before Rebound (and friends) ever see it.
///     Must only be added when <c>CardLocation</c> does NOT exist (see AutomatonMainFile).
/// </summary>
[HarmonyPatch]
public static class EncodeCardResultLocationOldPatch
{
    private static MethodBase? TargetMethod()
    {
        return AccessTools.Method(typeof(Hook), "ModifyCardPlayResultPileTypeAndPosition");
    }

    // Positional: matches (combatState, card, isAutoPlay, resources, pileType, position, out modifiers)
    // by index rather than by name, since only the first four names are confirmed (see
    // ModifyCardPlayResultLocationOldPatch's existing postfix, which already relies on them).
    private static void Prefix(CardModel card, ref PileType __4)
    {
        if (__4 != PileType.Discard) return;
        if (!AutomatonCmd.WillAutoEncode(card)) return;
        __4 = PileType.None;
    }
}
