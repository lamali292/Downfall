using System.Reflection;
using System.Reflection.Emit;
using BaseLib.Utils.Patching;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;

namespace Downfall.Code.Patches;

[HarmonyPatch]
public static class FromChooseACardScreenPatch
{
    private static MethodBase TargetMethod()
    {
        // Target the MoveNext method of the async state machine
        var stateMachine = typeof(CardSelectCmd).GetNestedTypes(AccessTools.all)
            .First(t => t.Name.Contains("FromChooseACardScreen"));
        return AccessTools.Method(stateMachine, "MoveNext");
    }

    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        return (List<CodeInstruction>)new InstructionPatcher(instructions)
            .Match(new InstructionMatcher()
                .opcode(OpCodes.Ldstr) // "Only works with less than 3 cards"
                .opcode(OpCodes.Ldstr) // "cards"
                .opcode(OpCodes.Newobj) // new ArgumentException
                .opcode(OpCodes.Throw))
            .ReplaceLastMatch([
                new CodeInstruction(OpCodes.Nop),
                new CodeInstruction(OpCodes.Nop),
                new CodeInstruction(OpCodes.Nop),
                new CodeInstruction(OpCodes.Nop)
            ]);
    }
}
