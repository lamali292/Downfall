using System.Reflection;
using BaseLib.Config;
using Downfall.DownfallCode.Abstract;
using Downfall.DownfallCode.Config;
using Downfall.DownfallCode.Nodes;
using Downfall.DownfallCode.Patches;
using Godot;
using Godot.Bridge;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using Logger = MegaCrit.Sts2.Core.Logging.Logger;

namespace Downfall.DownfallCode;

[ModInitializer(nameof(Initialize))]
public partial class DownfallMainFile : Node
{
    public const string ModId = "Downfall"; //At the moment, this is used only for the Logger and harmony names.

    public static Logger Logger { get; } =
        new(ModId, LogType.Generic);

    public static void Initialize()
    {
        ModConfigRegistry.Register(ModId, new DownfallConfig());
        Harmony harmony = new(ModId);

        var assembly = Assembly.GetExecutingAssembly();
        ScriptManagerBridge.LookupScriptsInAssembly(assembly);
        harmony.PatchAll();

        NCustomCardHolder.InitPool();
    }
}

[HarmonyPatch(typeof(ModelDb), "InitIds")]
internal static class ModelDbInitIdsPatch
{
    [HarmonyPostfix]
    private static void LogRegisteredCounts()
    {
        var modAssembly = typeof(DownfallMainFile).Assembly;
        var characters = ModelDb.AllCharacters
            .Where(c => c.GetType().Assembly == modAssembly)
            .ToList();
        foreach (var character in characters.OrderBy(c => c.Id.Entry))
        {
            var charName = character.GetType().Name;
            var cards = ModelDb.AllCards.Count(c => c.Pool == character.CardPool);
            var relics = ModelDb.AllRelics.Count(r => r.Pool == character.RelicPool);
            var potions = ModelDb.AllPotions.Count(p => p.Pool == character.PotionPool);
            DownfallMainFile.Logger.Info($"{charName}: {cards} cards, {relics} relics, {potions} potions");
        }

        var powers = ModelDb.AllPowers.Count(p => p.GetType().Assembly == modAssembly);
        DownfallMainFile.Logger.Info($"Powers: {powers}");

        foreach (var character in ModelDb.AllCharacters.OfType<DownfallCharacterModel>())
            if (character.CharacterSelectSfxEntry is { } effect)
                SfxOverridePatch.Register(character.CharacterSelectSfx, effect);
    }
}

/*

[HarmonyPatch(typeof(Log), nameof(Log.Error))]
public static class LogErrorPatch
{
    [HarmonyPrefix]
    public static bool DowngradeLocErrors(string text)
    {
        return !text.StartsWith("Localization formatting error!");
    }
}*/