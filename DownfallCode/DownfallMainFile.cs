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

        // SlimeBoss is an internal submod (ADR 0003): its own assembly (SlimeBossCode compiled
        // into SlimeBoss.dll by SlimeBoss.csproj) for code-separation, with no manifest/ModId of
        // its own, so the game's mod loader never discovers or calls it - Downfall's own MainFile
        // is responsible for calling into it directly.
        //
        // "SlimeBossBeta" doesn't exist yet - this is the forward-declared replacement ModId a
        // future SlimeBoss Beta standalone submod will use. If it's ever loaded alongside this
        // internal SlimeBoss, skip calling into it entirely so the two never both register the
        // same model IDs.
        if (!ReplaceableSubmod.IsSupersededBy("SlimeBossBeta"))
            InitializeSlimeBoss();

        // Automaton is an internal submod (ADR 0003), same pattern as SlimeBoss above.
        // "AutomatonBeta" doesn't exist yet - forward-declared replacement ModId, same rationale as
        // "SlimeBossBeta" above.
        if (!ReplaceableSubmod.IsSupersededBy("AutomatonBeta"))
            InitializeAutomaton();

        // Hermit is an internal submod (ADR 0003), same pattern as SlimeBoss/Automaton above.
        // "HermitBeta" doesn't exist yet - forward-declared replacement ModId, same rationale as
        // "SlimeBossBeta"/"AutomatonBeta" above.
        if (!ReplaceableSubmod.IsSupersededBy("HermitBeta"))
            InitializeHermit();
    }

    // Loaded by reflection, not a normal C# reference: SlimeBossCode needs DownfallCode's own
    // types (registries, ModPatcher, DownfallCardModel, ...) via SlimeBoss.csproj's
    // ProjectReference to this project, and a mutual ProjectReference between the two .csproj
    // files isn't something MSBuild/.NET supports - that would be a true circular dependency,
    // not just an awkward one. Keeping the ProjectReference in the direction that matters for
    // runtime correctness (SlimeBoss -> Downfall, so both share the one compiled copy of
    // DownfallCode's static registries - duplicating that source into SlimeBoss.dll instead would
    // silently split BundledSubmodLocRegistry/etc. into two independent, non-communicating
    // copies) means the Downfall -> SlimeBoss direction has to be a late-bound call instead.
    // SlimeBoss.dll/.pck are built as their own project (`dotnet build`/`dotnet publish
    // SlimeBoss.csproj`, see local.props.example) and copied next to Downfall.dll/.pck in the same
    // mod output folder by its own CopyToModsFolderOnBuild/GodotPublish targets, so they're always
    // sitting alongside whatever assembly this method itself was loaded from - that's resolved
    // here instead of relying on default assembly probing, since Downfall.dll itself was loaded by
    // the game's own AssemblyLoadContext from an arbitrary mod path, not the probing paths used
    // for the main app.
    //
    // Loading the dll alone is NOT enough for its [Pool]-attributed cards/powers/relics/character
    // to be discovered: BaseLib/the game's own content scanning (ReflectionHelper.ModTypes) only
    // walks types from assemblies ModManager has associated with a *loaded mod record*
    // (Mod.assemblies) - an assembly pulled in by Assembly.LoadFrom on its own is invisible to it,
    // which silently drops every SlimeBoss model (confirmed by a real test run: SlimeBoss's
    // character came back as "unknown" and nothing SlimeBoss-related registered). ModManager
    // exposes exactly this escape hatch for mods with secondary assemblies:
    // ModManager.AssociateAssemblyWithMod(modId, assembly) - call it before anything scans for
    // content (ModelDb.InitIds and earlier), associating SlimeBoss.dll with this mod's own
    // ("Downfall") id.
    //
    // Once SlimeBossMainFile.Initialize() runs, SlimeBoss's own Harmony patches are already
    // applied as part of it (via its own explicit ModPatcher.Create(...).Add(...).PatchAll()
    // calls - a per-type Harmony.CreateClassProcessor(...).Patch(), not an assembly scan - so
    // this doesn't depend on the game's automatic Harmony.PatchAll(assembly), which only covers
    // the manifest's own assembly (Downfall.dll) anyway).
    private static void InitializeSlimeBoss()
    {
        try
        {
            var downfallDir = Path.GetDirectoryName(typeof(DownfallMainFile).Assembly.Location) ?? "";

            var slimeBossPckPath = Path.Combine(downfallDir, "SlimeBoss.pck");
            if (File.Exists(slimeBossPckPath))
            {
                if (!ProjectSettings.LoadResourcePack(slimeBossPckPath))
                    Logger.Error($"Godot errored while loading SlimeBoss's resource pack at '{slimeBossPckPath}'.");
            }
            else
            {
                Logger.Error($"SlimeBoss.pck not found at '{slimeBossPckPath}' - its assets will not be available.");
            }

            var slimeBossPath = Path.Combine(downfallDir, "SlimeBoss.dll");
            if (!File.Exists(slimeBossPath))
            {
                Logger.Error($"SlimeBoss.dll not found at '{slimeBossPath}' - internal SlimeBoss submod will not be loaded.");
                return;
            }

            // Assembly.LoadFrom loads into its own default-load-context bucket, which does NOT
            // share already-resolved references with whatever context Downfall.dll itself was
            // loaded into (the game's ModManager loads mod assemblies via
            // AssemblyLoadContext.GetLoadContext(...).LoadFromAssemblyPath, not Assembly.LoadFrom
            // - see ModManager.TryLoadMod). Using Assembly.LoadFrom here caused SlimeBoss.dll's own
            // "BaseLib" reference to fail resolving (BaseLib.dll was already loaded, but into the
            // OTHER context), spamming FileNotFoundException and effectively hanging startup -
            // confirmed by an actual test run. Loading into the SAME context Downfall.dll lives in
            // lets SlimeBoss.dll's references resolve against what's already loaded there.
            var loadContext = AssemblyLoadContext.GetLoadContext(typeof(DownfallMainFile).Assembly);
            var slimeBossAssembly = loadContext != null
                ? loadContext.LoadFromAssemblyPath(slimeBossPath)
                : Assembly.LoadFrom(slimeBossPath);
            ModManager.AssociateAssemblyWithMod(ModId, slimeBossAssembly);

            var slimeBossMainFile = slimeBossAssembly.GetType("SlimeBoss.SlimeBossCode.SlimeBossMainFile");
            var initialize = slimeBossMainFile?.GetMethod("Initialize", BindingFlags.Public | BindingFlags.Static);
            if (initialize == null)
            {
                Logger.Error("Loaded SlimeBoss.dll but could not find SlimeBoss.SlimeBossCode.SlimeBossMainFile.Initialize() via reflection.");
                return;
            }

            initialize.Invoke(null, null);
        }
        catch (Exception ex)
        {
            Logger.Error($"Failed to initialize the internal SlimeBoss submod:\n{ex}");
        }
    }

    // Same recipe as InitializeSlimeBoss() above - see its doc comment for the full rationale
    // (ALC-based loading, AssociateAssemblyWithMod, why this can't be a normal ProjectReference).
    private static void InitializeAutomaton()
    {
        try
        {
            var downfallDir = Path.GetDirectoryName(typeof(DownfallMainFile).Assembly.Location) ?? "";

            var automatonPckPath = Path.Combine(downfallDir, "Automaton.pck");
            if (File.Exists(automatonPckPath))
            {
                if (!ProjectSettings.LoadResourcePack(automatonPckPath))
                    Logger.Error($"Godot errored while loading Automaton's resource pack at '{automatonPckPath}'.");
            }
            else
            {
                Logger.Error($"Automaton.pck not found at '{automatonPckPath}' - its assets will not be available.");
            }

            var automatonPath = Path.Combine(downfallDir, "Automaton.dll");
            if (!File.Exists(automatonPath))
            {
                Logger.Error($"Automaton.dll not found at '{automatonPath}' - internal Automaton submod will not be loaded.");
                return;
            }

            var loadContext = AssemblyLoadContext.GetLoadContext(typeof(DownfallMainFile).Assembly);
            var automatonAssembly = loadContext != null
                ? loadContext.LoadFromAssemblyPath(automatonPath)
                : Assembly.LoadFrom(automatonPath);
            ModManager.AssociateAssemblyWithMod(ModId, automatonAssembly);

            var automatonMainFile = automatonAssembly.GetType("Automaton.AutomatonCode.AutomatonMainFile");
            var initialize = automatonMainFile?.GetMethod("Initialize", BindingFlags.Public | BindingFlags.Static);
            if (initialize == null)
            {
                Logger.Error("Loaded Automaton.dll but could not find Automaton.AutomatonCode.AutomatonMainFile.Initialize() via reflection.");
                return;
            }

            initialize.Invoke(null, null);
        }
        catch (Exception ex)
        {
            Logger.Error($"Failed to initialize the internal Automaton submod:\n{ex}");
        }
    }

    // Same recipe as InitializeSlimeBoss()/InitializeAutomaton() above.
    private static void InitializeHermit()
    {
        try
        {
            var downfallDir = Path.GetDirectoryName(typeof(DownfallMainFile).Assembly.Location) ?? "";

            var hermitPckPath = Path.Combine(downfallDir, "Hermit.pck");
            if (File.Exists(hermitPckPath))
            {
                if (!ProjectSettings.LoadResourcePack(hermitPckPath))
                    Logger.Error($"Godot errored while loading Hermit's resource pack at '{hermitPckPath}'.");
            }
            else
            {
                Logger.Error($"Hermit.pck not found at '{hermitPckPath}' - its assets will not be available.");
            }

            var hermitPath = Path.Combine(downfallDir, "Hermit.dll");
            if (!File.Exists(hermitPath))
            {
                Logger.Error($"Hermit.dll not found at '{hermitPath}' - internal Hermit submod will not be loaded.");
                return;
            }

            var loadContext = AssemblyLoadContext.GetLoadContext(typeof(DownfallMainFile).Assembly);
            var hermitAssembly = loadContext != null
                ? loadContext.LoadFromAssemblyPath(hermitPath)
                : Assembly.LoadFrom(hermitPath);
            ModManager.AssociateAssemblyWithMod(ModId, hermitAssembly);

            var hermitMainFile = hermitAssembly.GetType("Hermit.HermitCode.HermitMainFile");
            var initialize = hermitMainFile?.GetMethod("Initialize", BindingFlags.Public | BindingFlags.Static);
            if (initialize == null)
            {
                Logger.Error("Loaded Hermit.dll but could not find Hermit.HermitCode.HermitMainFile.Initialize() via reflection.");
                return;
            }

            initialize.Invoke(null, null);
        }
        catch (Exception ex)
        {
            Logger.Error($"Failed to initialize the internal Hermit submod:\n{ex}");
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