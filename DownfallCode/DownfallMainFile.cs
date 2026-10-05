using System.Reflection;
using System.Runtime.Loader;
using BaseLib.Config;
using BaseLib.Patches.Features;
using BaseLib.Patches.Saves;
using BaseLib.Utils;
using Downfall.DownfallCode.Abstract;
using Downfall.DownfallCode.Audio;
using Downfall.DownfallCode.Config;
using Downfall.DownfallCode.CustomEnums;
using Downfall.DownfallCode.Data;
using Downfall.DownfallCode.Localization;
using Downfall.DownfallCode.Nodes;
using Downfall.DownfallCode.Patches;
using Downfall.DownfallCode.Utils;
using Downfall.DownfallCode.Voting;
using Godot;
using Godot.Bridge;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves.Runs;
using Logger = MegaCrit.Sts2.Core.Logging.Logger;

namespace Downfall.DownfallCode;

[ModInitializer(nameof(Initialize))]
public static class DownfallMainFile
{
    public const string ModId = "Downfall"; //At the moment, this is used only for the Logger and harmony names.

    public static Logger Logger { get; } =
        new(ModId, LogType.Generic);

    public static void Initialize()
    {
        BundledSubmodLocRegistry.Register(ModId);
        PostInitRegistry.Register(PostModelInit);
        CustomLocTableManager.Register("card_modifiers");
        CustomLocTableManager.Register("artists");
        CustomLocTableManager.Register("voting_ui");
        ExtendedSaveTypes.RegisterListSaveType<SerializableCard>();
        ModConfigRegistry.Register(ModId, new DownfallConfig());

        ScriptManagerBridge.LookupScriptsInAssembly(Assembly.GetExecutingAssembly());
        DownfallPatchManager.HarmonyPatches();
        //Patch(Assembly.GetExecutingAssembly(), ModId);


        NCustomCardHolder.InitPool();
        ModManager.OnMetricsUpload += DownfallMetrics.OnMetricsUpload;

        CardTitleHooks.Register((card, title) =>
        {
            if (!card.IsEcho) return title;
            var echoLoc = new LocString("card_keywords", "DOWNFALL-ECHO.card_title");
            echoLoc.Add("card", title);
            return echoLoc.GetFormattedText();
        });
        
        
        
        MainMenuButtonRegistry.Register(new MainMenuButtonRegistry.Entry
        {
            Label = "Art Voting", // fallback if the loc table entry is somehow missing
            LocLabel = new LocString("main_menu_ui", "DOWNFALL-ART_SUBMISSION"),
            IsVisible = () => DownfallConfig.ShowArtSubmissionButton,
            SubmenuType = typeof(NArtVotingScreen),
            CreateSubmenu = NArtVotingScreen.Create,
            OnPress = stack =>
            {
                stack?.PushSubmenuType<NArtVotingScreen>();
            }
        });

        // mention downfall sts1 credits somewhere
        ModCredits.Register(ModId,
            new ModCredits.Section("TEAM", ModCredits.Layout.Roles),
            new ModCredits.Section("HELP", ModCredits.Layout.Roles),
            new ModCredits.Section("ART"),
            new ModCredits.Section("SOUND"),
            new ModCredits.Section("LOC", Children:
            [
                new ModCredits.Section("LOC_ZHS"),
                new ModCredits.Section("LOC_FRA"),
                new ModCredits.Section("LOC_ITA"),
                new ModCredits.Section("LOC_RUS"),
                new ModCredits.Section("LOC_KOR"),
                //     new ModCredits.Section("LOC_PTB"),
                //     new ModCredits.Section("LOC_DEU"),
                new ModCredits.Section("LOC_JPN")
            ]),
            new ModCredits.Section("STS1")
        );
        //FmodStudioDeferredBankRegistration.RegisterBank("res://Downfall/audio/Master.bank");
        FmodStudio.RegisterBank("res://Downfall/audio/Master.strings.bank");
        FmodStudio.RegisterBank("res://Downfall/audio/Downfall.bank");
        
        List<string> list = ["Automaton", "Awakened", "Champ", "SlimeBoss", "Hermit", "Guardian", "Snecko", "Hexaghost"];
        foreach (var se in list)
        {
            InitializeSubmod(se);
        }
    }

    private static void InitializeSubmod(string name)
    {
        if (!ReplaceableSubmod.IsSupersededBy($"{name}Beta"))
            InitializeSubmod(name,
                $"{name}.{name}Code.{name}MainFile");
    }
    
    private static void InitializeSubmod(string name, string mainTypeName)
    {
        try
        {
            var downfallDir = Path.GetDirectoryName(typeof(DownfallMainFile).Assembly.Location) ?? "";

            var pckPath = Path.Combine(downfallDir, $"{name}.pck");
            if (File.Exists(pckPath))
            {
                if (!ProjectSettings.LoadResourcePack(pckPath))
                    Logger.Error($"Godot errored while loading {name}'s resource pack at '{pckPath}'.");
            }
            else
            {
                Logger.Error($"{name}.pck not found at '{pckPath}' - its assets will not be available.");
            }

            var dllPath = Path.Combine(downfallDir, $"{name}.dll");
            if (!File.Exists(dllPath))
            {
                Logger.Error($"{name}.dll not found at '{dllPath}' - internal {name} submod will not be loaded.");
                return;
            }

            var loadContext = AssemblyLoadContext.GetLoadContext(typeof(DownfallMainFile).Assembly);
            var assembly = loadContext != null
                ? loadContext.LoadFromAssemblyPath(dllPath)
                : Assembly.LoadFrom(dllPath);

            ModManager.AssociateAssemblyWithMod(ModId, assembly);

            var mainFile = assembly.GetType(mainTypeName);
            var initialize = mainFile?.GetMethod(
                "Initialize",
                BindingFlags.Public | BindingFlags.Static);

            if (initialize == null)
            {
                Logger.Error(
                    $"Loaded {name}.dll but could not find {mainTypeName}.Initialize() via reflection.");
                return;
            }

            initialize.Invoke(null, null);
        }
        catch (Exception ex)
        {
            Logger.Error($"Failed to initialize the internal {name} submod:\n{ex}");
        }
    }

    private static void PostModelInit()
    {
        CustomTargetType.RegisterMultiTargetType(DownfallTargetType.MeAndEnemies,
            (target, player) =>
                target is { IsAlive: true, IsPet: false, IsEnemy: true } || target == player.Creature);
        LogRegisteredCounts();
        CustomPowerInstanceType.RegisterAll();
    }

    public static string GetDownfallVersion()
    {
        var mod = ModManager.GetLoadedMods().FirstOrDefault(m => m.manifest?.id == "Downfall");

        return mod?.manifest?.version ?? "unknown";
    }


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
            Logger.Info($"{charName}: {cards} cards, {relics} relics, {potions} potions");
        }

        var powers = ModelDb.AllPowers.Count(p => p.GetType().Assembly == modAssembly);
        Logger.Info($"Powers: {powers}");
    }
}