using System.Reflection;
using System.Reflection.Emit;
using BaseLib.Utils.Patching;
using Downfall.DownfallCode.Interfaces;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Patches;

/// <summary>
/// <c>CombatManager.ResolveTurnEndCardEffects</c> hardcodes moving a <c>HasTurnEndInHandEffect</c> card to the
/// Discard pile once its turn-end effect finishes (unless it's Ethereal, which exhausts instead). Cards that
/// implement <see cref="IReturnsToHandAfterTurnEnd"/> need to end up back in Hand instead, and there's no hook
/// for that, so this redirects the hardcoded Discard-pile add to a Hand-pile add for those cards.
/// <para>
///     Patched on the async state machine's <c>MoveNext</c>, not on <c>ResolveTurnEndCardEffects</c> itself:
///     an async method's kickoff stub is tiny and gets inlined into callers, which silently skips a Harmony
///     prefix/postfix placed on it.
/// </para>
/// </summary>
[HarmonyPatch]
internal static class ReturnToHandAfterTurnEndPatch
{
    private static readonly Type StateMachineType = typeof(CombatManager)
        .GetNestedTypes(AccessTools.all)
        .FirstOrDefault(t => t.Name.Contains("ResolveTurnEndCardEffects"))
        ?? throw new MissingMemberException("CombatManager.ResolveTurnEndCardEffects state machine not found");

    private static readonly FieldInfo CardField = AccessTools.Field(StateMachineType, "card");

    private static readonly MethodInfo OwnerGetter = AccessTools.PropertyGetter(typeof(CardModel), nameof(CardModel.Owner));

    private static readonly MethodInfo GetPileMethod = AccessTools.Method(
        typeof(PileTypeExtensions), nameof(PileTypeExtensions.GetPile));

    private static readonly MethodInfo CardPileCmdAddMethod = AccessTools.Method(
        typeof(CardPileCmd), nameof(CardPileCmd.Add),
        [typeof(CardModel), typeof(CardPile), typeof(CardPilePosition), typeof(AbstractModel), typeof(bool)]);

    private static readonly MethodInfo GetAwaiterMethod = AccessTools.Method(
        typeof(Task<CardPileAddResult>), nameof(Task<CardPileAddResult>.GetAwaiter));

    private static readonly MethodInfo ReplacementMethod = AccessTools.Method(
        typeof(ReturnToHandReplacement), nameof(ReturnToHandReplacement.AddToHandInstead));

    private static readonly MethodInfo ShouldReturnToHandMethod = AccessTools.Method(
        typeof(ReturnToHandReplacement), nameof(ReturnToHandReplacement.ShouldReturnToHand));

    private static MethodBase TargetMethod() => AccessTools.Method(StateMachineType, "MoveNext");

    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        var patcher = new InstructionPatcher(instructions);

        patcher.Match(
            new InstructionMatcher()
                .ldarg_0()
                .ldfld(CardField)
                .ldc_i4_3() // PileType.Discard
                .ldarg_0()
                .ldfld(CardField)
                .callvirt(OwnerGetter)
                .call_any(GetPileMethod)
                .ldc_i4_1() // CardPilePosition.Bottom
                .ldnull() // clonedBy
                .ldc_i4_1() // skipVisuals: true
                .call_any(CardPileCmdAddMethod)
                .call_any(GetAwaiterMethod)
        );

        var labelReplacement = generator.DefineLabel();
        var labelEnd = generator.DefineLabel();

        // The instructions we're inserting before are the target of the Ethereal check's forward branch
        // (`brfalse.s` when NOT Ethereal), not just something execution falls through into. InsertBeforeMatch
        // only shifts the matched instructions down the list - it does NOT move the incoming branch's label,
        // which stays attached to the (now-shifted) first matched instruction. Without re-homing that label
        // onto our new first instruction, the branch jumps straight past our check and this patch is a no-op.
        patcher.InsertBeforeMatch(new List<CodeInstruction>
        {
            new(OpCodes.Ldarg_0),
            new(OpCodes.Ldfld, CardField),
            new(OpCodes.Call, ShouldReturnToHandMethod),
            new(OpCodes.Brtrue, labelReplacement),
        });

        // _index now sits exactly on the (shifted) original first matched instruction (ldarg.0).
        patcher.TakeLabels(out var incomingLabels);
        var insertedCode = (List<CodeInstruction>)patcher;
        insertedCode[patcher.Index - 4].labels.AddRange(incomingLabels);

        patcher.Step(12); // original 12 matched instructions, untouched

        patcher.Insert(new CodeInstruction(OpCodes.Br, labelEnd));
        patcher.Insert(new CodeInstruction(OpCodes.Ldarg_0).WithLabels(labelReplacement));
        patcher.Insert(new CodeInstruction(OpCodes.Ldfld, CardField));
        patcher.Insert(new CodeInstruction(OpCodes.Call, ReplacementMethod));
        patcher.Insert(new CodeInstruction(OpCodes.Callvirt, GetAwaiterMethod)); // replacement path also produces TaskAwaiter<CardPileAddResult>

        patcher.GetInstruction(out var nextInstruction);
        nextInstruction.labels.Add(labelEnd);

        return (List<CodeInstruction>)patcher;
    }
}

internal static class ReturnToHandReplacement
{
    public static bool ShouldReturnToHand(CardModel card) => card is IReturnsToHandAfterTurnEnd;

    // Only ever called for cards implementing IReturnsToHandAfterTurnEnd; never touches the Discard pile at all.
    public static Task<CardPileAddResult> AddToHandInstead(CardModel card) =>
        CardPileCmd.Add(card, PileType.Hand.GetPile(card.Owner), CardPilePosition.Bottom, null, skipVisuals: true);
}
