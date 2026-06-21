using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Utils;

public static class CardExecutionRegistry
{
    public delegate Task AfterPlayCallback(CardModel card, PlayerChoiceContext choiceContext, CardPlay cardPlay);

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
    
    public static async Task<bool> BeforeOnPlayInternal(CardModel card, PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        foreach (var cb in BeforeListeners)
            if (!await cb(card, choiceContext, cardPlay))
                return true;
        return false;
    }

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