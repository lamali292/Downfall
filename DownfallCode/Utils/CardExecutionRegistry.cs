using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Utils;

public static class CardExecutionRegistry
{
    public delegate Task AfterPlayCallback(CardModel card, PlayerChoiceContext choiceContext, CardPlay cardPlay);

    /// <summary>
    ///     A "before play" listener. Return <c>true</c> to ALLOW the play to proceed,
    ///     or <c>false</c> to CANCEL it.
    ///     All before-listeners always run, regardless of what others return — a cancel
    ///     vote does NOT skip the remaining listeners. So it is safe to record state here
    ///     (e.g. snapshots) without worrying about registration order.
    /// </summary>
    public delegate Task<bool> BeforePlayCallback(CardModel card, PlayerChoiceContext choiceContext, CardPlay cardPlay);

    internal static readonly List<AfterPlayCallback> AfterListeners = [];
    internal static readonly List<BeforePlayCallback> BeforeListeners = [];


    public static void RegisterBefore(BeforePlayCallback callback)
    {
        if (!BeforeListeners.Contains(callback)) BeforeListeners.Add(callback);
    }

    public static void RegisterAfter(AfterPlayCallback callback)
    {
        if (!AfterListeners.Contains(callback)) AfterListeners.Add(callback);
    }

    /// <summary>
    ///     Runs ALL before-listeners (none are skipped, even if an earlier one votes to cancel).
    ///     Returns <c>true</c> if the play should be CANCELLED (at least one listener returned <c>false</c>),
    ///     or <c>false</c> if the play should PROCEED (every listener returned <c>true</c>).
    ///     Caller contract: <c>if (BeforeOnPlayInternal(...)) return;</c>
    /// </summary>
    public static async Task<bool> BeforeOnPlayInternal(CardModel card, PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var cancel = false;
        foreach (var cb in BeforeListeners)
            if (!await cb(card, ctx, cardPlay))
                cancel = true;
        return cancel;
    }

    /// <summary>
    ///     Runs all after-listeners in order. Only reached if the play was NOT canceled
    ///     by a before-listener. After-listeners cannot affect whether the play happens.
    /// </summary>
    public static async Task AfterOnPlayInternal(CardModel card, PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        foreach (var cb in AfterListeners)
            await cb(card, choiceContext, cardPlay);
    }
}

/*

[HarmonyPatch(typeof(CardModel), "OnPlayWrapper", MethodType.Async)]
public static class MasterPatchOnPlayWrapper
{
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Transpiler(
        IEnumerable<CodeInstruction> instructions,
        ILGenerator generator,
        MethodBase original)
    {
        var OnPlayInternalMethod = AccessTools.Method(typeof(CardModel), "OnPlay")
                           ?? throw new Exception("Registry Error: Could not find CardModel.OnPlay");

        var code = AsyncMethodCall.Create(generator, instructions, original,
            AccessTools.Method(typeof(MasterPatchOnPlayWrapper), nameof(CardExecutionRegistry.BeforeOnPlayInternal)),
            OnPlayInternalMethod,
            resultName: "returnIf");

        code = AsyncMethodCall.Create(generator, code, original,
            AccessTools.Method(typeof(MasterPatchOnPlayWrapper), nameof(CardExecutionRegistry.AfterOnPlayInternal)),
            afterState: OnPlayInternalMethod);

        return code;
    }
}
*/