using System.Reflection;
using System.Reflection.Emit;
using BaseLib.Utils.Patching;
using Downfall.Code.Cards.CardModels;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using FileAccess = Godot.FileAccess;

namespace Downfall.Code.Patches;

[HarmonyPatch(typeof(LocManager), "ListLocalizationFiles")]
public static class ListLocalizationFilesPatch
{
    private static readonly string[] ExtraTables = ["encode.json"];

    public static void Postfix(ref IEnumerable<string> __result)
    {
        var result = __result;
        var enumerable = result as string[] ?? result.ToArray();
        __result = enumerable.Concat(ExtraTables.Where(f => !enumerable.Contains(f)));
    }
}

[HarmonyPatch(typeof(LocManager), "LoadTable")]
public static class LoadTablePatch
{
    public static bool Prefix(string path, ref Dictionary<string, string> __result)
    {
        if (FileAccess.FileExists(path)) return true;
        __result = new Dictionary<string, string>();
        return false;
    }
}

[HarmonyPatch]
public static class GetDescriptionForPilePatch
{
    private const int SourceLocalIndex = 5;
    private const string KeywordsTable = "card_keywords";

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
                    AccessTools.Method(typeof(GetDescriptionForPilePatch), nameof(InjectLines)))
            ]);
    }

    public static void InjectLines(CardModel card, List<string> source)
    {
        if (card is not AutomatonCardModel automaton) return;

        if (automaton is IEncodable { AutoEncode: true } encodable)
            AddEncodeLine(source, encodable);

        if (automaton is ICompilable)
            AddLine(source, ICompilable.BuildCompileLocString(automaton), "DOWNFALL-COMPILE.title");

        if (automaton is ICompilableError && !automaton.SuppressCompileError)
            AddLine(source, ICompilableError.BuildErrorLocString(automaton), "DOWNFALL-COMPILE_ERROR.title");
    }

    private static void AddEncodeLine(List<string> source, IEncodable encodable)
    {
        var encode = encodable.EncodeLocString;
        if (encode == null) return;

        var title = GetTitle("DOWNFALL-ENCODE.title");
        var insertIndex = source.FindIndex(l => !l.StartsWith("[gold]") || !l.EndsWith("."));
        source.Insert(insertIndex < 0 ? 0 : insertIndex, $"{encode.GetFormattedText()}\n[gold]{title}[/gold].");
    }

    private static void AddLine(List<string> source, LocString? loc, string titleKey)
    {
        if (loc == null) return;
        source.Add($"[gold]{GetTitle(titleKey)}[/gold] - {loc.GetFormattedText()}");
    }

    private static string GetTitle(string key)
    {
        return new LocString(KeywordsTable, key).GetFormattedText();
    }
}