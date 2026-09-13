using HarmonyLib;
using BaseLib.Utils.Patching;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using System.Reflection;
using System.Reflection.Emit;

namespace Collector.CollectorCode.Patches;

[HarmonyPatch]
public static class OnPlayWrapperPlayCountPatch
{
    private static readonly MethodInfo GeneratePlayCountMethod = AccessTools.Method(
        typeof(CardModel),
        nameof(CardModel.GeneratePlayCount),
        [typeof(ICombatState), typeof(Creature)]);

    private static readonly MethodInfo ReplacementMethod = AccessTools.Method(
        typeof(ReplayPlayCountSkip),
        nameof(ReplayPlayCountSkip.GetPlayCountForOnPlay));

    static MethodBase TargetMethod()
    {
        var stateMachineType = typeof(CardModel)
            .GetNestedTypes(AccessTools.all)
            .FirstOrDefault(t => t.Name.Contains("OnPlayWrapper"))
            ?? throw new Exception("Could not find OnPlayWrapper state machine type");

        return AccessTools.Method(stateMachineType, "MoveNext")
            ?? throw new Exception("Could not find MoveNext on OnPlayWrapper state machine");
    }

    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        var getAwaiterMethod = typeof(Task<int>).GetMethod(nameof(Task<int>.GetAwaiter));

        var patcher = new InstructionPatcher(instructions);

        patcher.Match(
            new InstructionMatcher()
                .ldloc_any()
                .ldarg_0()
                .ldfld()
                .ldarg_0()
                .ldfld()
                .call_any(GeneratePlayCountMethod)
                .call_any(getAwaiterMethod)  
        );

        patcher.CopyMatch(out var original);
        var loadCardModel = original[0];

        var labelReplacement = generator.DefineLabel();
        var labelEnd = generator.DefineLabel();

        patcher.InsertBeforeMatch(new List<CodeInstruction>
        {
            loadCardModel.Clone(),
            new(OpCodes.Dup),
            new(OpCodes.Isinst, typeof(ISkipReplayOnSelfExhaust)),
            new(OpCodes.Brtrue, labelReplacement),
            new(OpCodes.Pop),
        });

        patcher.Step(7); // now 7 instructions, GeneratePlayCount + GetAwaiter, untouched

        patcher.Insert(new CodeInstruction(OpCodes.Br, labelEnd));
        patcher.Insert(new CodeInstruction(OpCodes.Call, ReplacementMethod).WithLabels(labelReplacement));
        patcher.Insert(new CodeInstruction(OpCodes.Callvirt, getAwaiterMethod)); // replacement path also produces TaskAwaiter<int>

        patcher.GetInstruction(out var nextInstruction);
        nextInstruction.labels.Add(labelEnd);

        return (List<CodeInstruction>)patcher;
    }
}

public static class ReplayPlayCountSkip
{
    // Only ever called for marked cards never touches GeneratePlayCount at all.
    public static Task<int> GetPlayCountForOnPlay(CardModel card) => Task.FromResult(1);
}

public interface ISkipReplayOnSelfExhaust;