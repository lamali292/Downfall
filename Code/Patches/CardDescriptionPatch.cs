using System.Reflection;
using System.Reflection.Emit;
using BaseLib.Utils.Patching;
using Downfall.Code.Localization;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Patches;


[HarmonyPatch]
public static class CardDescriptionPatch
{
    private const int SourceLocalIndex = 5;

    private static MethodBase TargetMethod()
    {
        return AccessTools.Method(typeof(CardModel), "GetDescriptionForPile",
        [
            typeof(PileType),
            AccessTools.Inner(typeof(CardModel), "DescriptionPreviewType"),
            typeof(Creature)
        ]);
    }

    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        return (List<CodeInstruction>)new InstructionPatcher(instructions)
            .Match(new InstructionMatcher().ldloc_s(SourceLocalIndex).opcode(OpCodes.Ldsfld))
            .Insert([
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldloc_S, (byte)SourceLocalIndex),
                new CodeInstruction(OpCodes.Call,
                    AccessTools.Method(typeof(CardDescriptionPatch), nameof(InjectLines)))
            ]);
    }

    public static void InjectLines(CardModel card, List<string> source)
        => CardDescriptionRegistry.InjectLines(card, source);
}