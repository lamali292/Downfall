using HarmonyLib;
using MegaCrit.Sts2.Core.DevConsole;
using MegaCrit.Sts2.Core.DevConsole.ConsoleCommands;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Models.Relics;

namespace Downfall.DownfallCode.Patches;

[HarmonyPatch]
public static class AncientSeaGlassConsolePatch
{
    private const string SeaGlassKey = "SEA_GLASS";
    private const string Prefix = SeaGlassKey + "_";

    /// <summary>Character entry (e.g. "IRONCLAD") forced via console, or null for default behavior.</summary>
    private static string? _forcedSeaGlassCharacter;

    // 1) Rewrite "SEA_GLASS_IRONCLAD" -> "SEA_GLASS" + remember the character,
    //    so the vanilla Contains() validation and DebugOption matching still work.
    [HarmonyPrefix]
    [HarmonyPatch(typeof(AncientConsoleCmd), nameof(AncientConsoleCmd.Process))]
    private static void RewriteChoice(string[] args)
    {
        _forcedSeaGlassCharacter = null;
        if (args.Length <= 1 || !args[1].ToUpperInvariant().StartsWith(Prefix)) return;
        _forcedSeaGlassCharacter = args[1].ToUpperInvariant()[Prefix.Length..];
        args[1] = SeaGlassKey;
    }

    // 2) While a character is forced, SeaGlassOptions yields only that character's copy,
    //    so DebugOption("SEA_GLASS") can only match the one you asked for.
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Orobas), "SeaGlassOptions", MethodType.Getter)]
    private static bool ForceSeaGlassCharacter(Orobas __instance, ref IEnumerable<EventOption> __result)
    {
        if (_forcedSeaGlassCharacter == null)
            return true; // vanilla behavior

        var character = ModelDb.AllCharacters
            .FirstOrDefault(c => c.Id.Entry.Equals(_forcedSeaGlassCharacter, StringComparison.OrdinalIgnoreCase));
        if (character == null)
            return true;

        var glass = (SeaGlass)ModelDb.Relic<SeaGlass>().ToMutable();
        glass.CharacterId = character.Id;
        __result = [__instance.RelicOption(glass)];
        return false;
    }

    // 3) Completions: collapse the five duplicate SEA_GLASS entries into per-character suggestions.
    [HarmonyPrefix]
    [HarmonyPatch(typeof(AncientConsoleCmd), nameof(AncientConsoleCmd.GetArgumentCompletions))]
    private static bool ExpandCompletions(AncientConsoleCmd __instance, string[] args, ref CompletionResult __result)
    {
        if (args.Length != 2)
            return true;
        if (ModelDb.GetByIdOrNull<EventModel>(new ModelId(ModelDb.GetCategory(typeof(EventModel)),
                args[0].ToUpperInvariant())) is not Orobas orobas)
            return true;

        var names = new List<string>();
        foreach (var name in orobas.AllPossibleOptions.Select(o => o.TextKey.Split('.').Last()))
            if (name == SeaGlassKey)
            {
                if (!names.Any(n => n.StartsWith(Prefix)))
                    names.AddRange(ModelDb.AllCharacters.Select(c => Prefix + c.Id.Entry));
            }
            else if (!names.Contains(name))
            {
                names.Add(name);
            }

        __result = __instance.CompleteArgument(names, [args[0]], args[1]);
        return false;
    }
}