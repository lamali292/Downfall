using System.Reflection;
using BaseLib.Config;
using BaseLib.Patches.Features;
using BaseLib.Patches.Saves;
using BaseLib.Utils;
using Downfall.DownfallCode.Abstract;
using Downfall.DownfallCode.Audio;
using Downfall.DownfallCode.Config;
using Downfall.DownfallCode.CustomEnums;
using Downfall.DownfallCode.Data;
using Downfall.DownfallCode.Nodes;
using Downfall.DownfallCode.Patches;
using Downfall.DownfallCode.Utils;
using Downfall.DownfallCode.Voting;
using Godot.Bridge;
using MegaCrit.Sts2.Core.AutoSlay;
using MegaCrit.Sts2.Core.Helpers;
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
        PostInitRegistry.Register(PostModelInit);
        CustomLocTableManager.Register("card_modifiers");
        CustomLocTableManager.Register("artists");
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
            Label = "Auto Slay",
            IsVisible = () => DownfallConfig.DevMode,
            SubmenuType = null,
            CreateSubmenu = null,
            OnPress = stack =>
            {
                var slayer = new AutoSlayer();
                slayer.Start(SeedHelper.GetRandomSeed(), "autoslay.log");
            }
        });


        /*
        MainMenuButtonRegistry.Register(new MainMenuButtonRegistry.Entry
        {
            Label = "Art Voting",
            IsVisible = () => DownfallConfig.DevMode,
            SubmenuType = typeof(NArtVotingScreen),
            CreateSubmenu = NArtVotingScreen.Create,
            OnPress = stack =>
            {
                if (VotingApi.Instance == null)
                    stack?.GetTree().Root.AddChild(new VotingApi());
                stack?.PushSubmenuType<NArtVotingScreen>();
            }
        });*/

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
        FmodStudio.RegisterGuidMappings("res://Downfall/audio/GUIDs.txt");
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