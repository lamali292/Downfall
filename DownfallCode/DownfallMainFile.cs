using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Text.Json;
using BaseLib.Config;
using BaseLib.Extensions;
using BaseLib.Patches.Saves;
using BaseLib.Utils;
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
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Saves.Runs;
using HttpClient = System.Net.Http.HttpClient;
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
        CustomLocTableManager.Register("card_modifiers");
        CustomLocTableManager.Register("artists");
        ExtendedSaveTypes.RegisterListSaveType<SerializableCard>();
        ModConfigRegistry.Register(ModId, new DownfallConfig());
        
        Patch(Assembly.GetExecutingAssembly(), ModId);
  

        NCustomCardHolder.InitPool();
        ModManager.OnMetricsUpload += OnMetricsUpload;
        
    }

    private static readonly HashSet<Assembly> Patched = [];
    public static void Patch(Assembly assembly, string modid)
    {
        if (!Patched.Add(assembly)) return; 
        Harmony harmony = new(modid);
        ScriptManagerBridge.LookupScriptsInAssembly(assembly);
        harmony.TryPatchAll(assembly);
    }

    private static void OnMetricsUpload(SerializableRun run, bool isVictory, ulong localPlayerId)
    {
        if (!DownfallConfig.UploadMetrics) return;
        if (run.Players.All(e =>
                e.CharacterId == null ||
                ModelDb.GetById<CharacterModel>(e.CharacterId) is not DownfallCharacterModel)) return;
        var anonymized = run.Anonymized();
        var json = JsonSerializer.Serialize(anonymized);
        _ = SendToServer(json);
    }

    private static async Task SendToServer(string json)
    {
        var bytes = Encoding.UTF8.GetBytes(json);
        using var client = new HttpClient();
        client.Timeout = TimeSpan.FromSeconds(15);
        var content = new ByteArrayContent(bytes);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        try
        {
            // TODO
            var response = await client.PutAsync("http://localhost:3000/runs", content);
            if (response.IsSuccessStatusCode)
                Logger.Info("Upload successful!");
            else
                Logger.Warn($"Upload failed: {response.StatusCode}");
        }
        catch (HttpRequestException ex)
        {
            Logger.Warn($"Upload failed due to network error: {ex.Message}");
        }
        catch (TaskCanceledException ex)
        {
            Logger.Warn($"Upload timed out: {ex.Message}");
        }
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
                SfxOverrideRegistry.Register(character.CharacterSelectSfx, effect);
    }
}