using System.Reflection;
using BaseLib.Utils;
using HarmonyLib;
using Hermit.HermitCode.Core;
using MegaCrit.Sts2.Core.Models;

namespace Hermit.HermitCode.Patches;

/// <summary>
///     Snapshots a card's Dead On / curse-adjacency status the moment it starts playing,
///     while it is still sitting in the hand. Everything downstream (the after-play handler,
///     Combo, Headshot, ...) reads the snapshot because by then the card is in the Play pile
///     and its hand position is gone.
///     <para>
///         Patched on the async state machine's <c>MoveNext</c>, not on
///         <c>CardModel.OnPlayWrapper</c> itself: the kickoff stub of an async method is tiny
///         and gets inlined into callers (e.g. <c>CardCmd.AutoPlay</c>), which silently skips a
///         Harmony prefix placed on it.
///     </para>
/// </summary>
[HarmonyPatch]
internal static class DeadOnPatch
{
    private static readonly Type StateMachineType = typeof(CardModel)
        .GetNestedTypes(AccessTools.all)
        .FirstOrDefault(t => t.Name.Contains("OnPlayWrapper"))
        ?? throw new MissingMemberException("CardModel.OnPlayWrapper state machine not found");

    private static readonly FieldInfo StateField = AccessTools.Field(StateMachineType, "<>1__state");
    private static readonly FieldInfo ThisField = AccessTools.Field(StateMachineType, "<>4__this");

    // Keyed per card so simultaneous plays (multiplayer) can't overwrite each other's snapshot.
    private static readonly SpireField<CardModel, bool> WasDeadOn = new(() => false);
    private static readonly SpireField<CardModel, bool> WasAdjacentToCurse = new(() => false);

    internal static bool WasPlayedDeadOn(CardModel card) => WasDeadOn[card];
    internal static bool WasPlayedAdjacentToCurse(CardModel card) => WasAdjacentToCurse[card];

    private static MethodBase TargetMethod() => AccessTools.Method(StateMachineType, "MoveNext");

    private static void Prefix(object __instance)
    {
        // MoveNext runs once per await; only the first step (state -1) still has the card in hand.
        if ((int)StateField.GetValue(__instance)! != -1) return;
        var card = (CardModel)ThisField.GetValue(__instance)!;
        WasDeadOn[card] = HermitCmd.IsDeadOnInCurrentHandState(card);
        WasAdjacentToCurse[card] = HermitCmd.IsAdjacentToCurseInCurrentHandState(card);
    }
}
