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
    private static readonly SpireField<CardModel, PlayStartHandStatus> Status = new(() => PlayStartHandStatus.None);

    internal static PlayStartHandStatus StatusOf(CardModel card) => Status[card];

    /// <summary>
    ///     Snapshots hand status for a card that never goes through <c>OnPlayWrapper</c> but still
    ///     needs <see cref="HermitCmd.IsDeadOn" /> to work once it leaves the hand - currently only
    ///     <c>ImpendingDoom</c>, from its <c>HasTurnEndInHandEffect</c> getter (the last point the
    ///     engine reads while the card is still in the Hand pile; by the time <c>OnTurnEndInHand</c>
    ///     runs, the card has already been moved to the Play pile).
    /// </summary>
    internal static void CaptureNow(CardModel card) => Status[card] = HermitCmd.CaptureHandStatus(card);

    private static MethodBase TargetMethod() => AccessTools.Method(StateMachineType, "MoveNext");

    private static void Prefix(object __instance)
    {
        // MoveNext runs once per await; only the first step (state -1) still has the card in hand.
        if ((int)StateField.GetValue(__instance)! != -1) return;
        var card = (CardModel)ThisField.GetValue(__instance)!;
        // Hand-position only: IShouldTriggerDeadOn sources are re-checked live per replay
        // instance instead (see HermitCmd.IsDeadOn), since their answer can change
        // across a single card's own replay instances while a stale snapshot here cannot.
        Status[card] = HermitCmd.CaptureHandStatus(card);
    }
}
