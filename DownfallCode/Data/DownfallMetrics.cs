using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.Debug;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Platform;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Runs.History;
using MegaCrit.Sts2.Core.Runs.Metrics;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace Downfall.DownfallCode.Data;

public static class DownfallMetrics
{
    internal static void OnMetricsUpload(SerializableRun run, bool isVictory, ulong localPlayerId)
    {
        if (!ShouldUpload(run, localPlayerId)) return;
        var metrics = GetRunMetrics(run, isVictory, localPlayerId);
        var json = JsonSerializer.Serialize<RunMetrics>(metrics, MetricsSerializerContext.Default.RunMetrics);
        _ = SendToServer(json);
    }


    private static bool ShouldUpload(SerializableRun run, ulong localPlayerId)
    {
        if (!MetricUtilities.ShouldUploadMetrics()) return false;
        if (RunManager.Instance.IsAbandoned)
        {
            DownfallMainFile.Logger.Info("Skipping metrics upload, run was abandoned.");
            return false;
        }
        if (run.GameMode != GameMode.Standard)
        {
            DownfallMainFile.Logger.Info("Skipping metrics upload, custom mode detected.");
            return false;
        }
        if (run.MapPointHistory.SelectMany(logs => logs).Count() < 5)
        {
            DownfallMainFile.Logger.Info("Skipping metrics upload, not enough progress.");
            return false;
        }
        if (run.Players.All(e =>
                e.CharacterId == null ||
                ModelDb.GetById<CharacterModel>(e.CharacterId) is not DownfallCharacterModel))
        {
            DownfallMainFile.Logger.Info("Skipping metrics upload, no downfall character found active.");
            return false;
        }
        if (run.Players.First(p => (long)p.NetId == (long)localPlayerId).CharacterId == null)
        {
            DownfallMainFile.Logger.Info("Skipping metrics upload, no local player found.");
            return false;
        }
        return true;
    }


    private const string SupabaseUrl = "https://wxememsxgrgrfvntulgr.supabase.co/rest/v1/runs";
    private const string AnonKey = "sb_publishable_XJRuWuyy0fJwKVFUQ8L3Dw_JrokGm_i";

    private static async Task SendToServer(string json)
    {
        try
        {
            DownfallMainFile.Logger.Info("Start uploading Metrics!");

            var version = DownfallMainFile.GetDownfallVersion();
            var wrapped = $"{{\"mod_version\":{JsonSerializer.Serialize(version)},\"data\":{json}}}";
            var bytes = Encoding.UTF8.GetBytes(wrapped);

            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(15);

            using var content = new ByteArrayContent(bytes);
            content.Headers.ContentType =
                new MediaTypeHeaderValue("application/json");

            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                SupabaseUrl);

            request.Content = content;

            request.Headers.Add("apikey", AnonKey);
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", AnonKey);
            request.Headers.Add("Prefer", "return=minimal");

            DownfallMainFile.Logger.Info("Before PostAsync!");

            var response = await client.SendAsync(request);

            var responseBody = await response.Content.ReadAsStringAsync();

            DownfallMainFile.Logger.Info(
                $"PostAsync returned: {(int)response.StatusCode} {response.StatusCode}");

            if (response.IsSuccessStatusCode)
            {
                DownfallMainFile.Logger.Info(
                    $"Upload successful! {responseBody}");
            }
            else
            {
                DownfallMainFile.Logger.Warn(
                    $"Upload failed: {response.StatusCode} {responseBody}");
            }
        }
        catch (Exception ex)
        {
            DownfallMainFile.Logger.Error(
                $"Metrics upload exception: {ex}");
        }
        finally
        {
            DownfallMainFile.Logger.Info("End uploading Metrics!");
        }
    }
    
    private static RunMetrics GetRunMetrics(
        SerializableRun run,
        bool isVictory,
        ulong localPlayerId)
    {
        LocManager.Instance.StartOverridingLanguageAsEnglish();
        try
        {
            var modelId = ModelId.none;
            var source = run.MapPointHistory.LastOrDefault();
            var pointHistoryEntry =
                source?.LastOrDefault();
            if (!isVictory && pointHistoryEntry != null &&
                pointHistoryEntry.Rooms.Last().RoomType.IsCombatRoom())
                modelId = pointHistoryEntry.Rooms.Last().ModelId;
            var localPlayer =
                run.Players.First(p => (long)p.NetId == (long)localPlayerId);
            var list1 = run.MapPointHistory
                .SelectMany(logs =>
                    logs).ToList();
            var list2 = list1
                .Where(e =>
                    e.Rooms.Last().RoomType.IsCombatRoom())
                .Select(e =>
                    new EncounterMetric((e.Rooms.Last().ModelId ?? ModelId.none).Entry,
                        int.Min(e.GetEntry(localPlayerId).DamageTaken, localPlayer.MaxHp),
                        e.Rooms.Last().TurnsTaken + 1)).ToList();
            var list3 = list1
                .Where(
                    (Func<MapPointHistoryEntry, bool>)(e => e.GetEntry(localPlayerId).CardChoices.Count > 0))
                .Select(
                    (Func<MapPointHistoryEntry, CardChoiceMetric>)(e =>
                        new CardChoiceMetric(e.GetEntry(localPlayerId).CardChoices))).ToList();
            var list4 = list1
                .Where(
                    (Func<MapPointHistoryEntry, bool>)(e => e.MapPointType == MapPointType.Ancient))
                .Where(e => e.GetEntry(localPlayerId).AncientChoices.Count > 0)
                .Select(e => new AncientMetric(e, e.GetEntry(localPlayerId)))
                .ToList();

            var actWinMetricList = new List<ActWinMetric>();
            var eventChoiceMetricList = new List<EventChoiceMetric>();
            for (var index = 0; index < run.MapPointHistory.Count; ++index)
            {
                eventChoiceMetricList.AddRange(from entry in run.MapPointHistory[index]
                    where entry.Rooms.First().RoomType == RoomType.Event &&
                          entry.GetEntry(localPlayerId).EventChoices.Count != 0 &&
                          entry.MapPointType != MapPointType.Ancient
                    select new EventChoiceMetric(entry, localPlayerId, run.Acts[index]));

                var win = index < run.MapPointHistory.Count - 1 | isVictory;
                var actEntry = run.Acts[index].Id?.Entry;
                if (actEntry == null) continue;
                actWinMetricList.Add(new ActWinMetric(actEntry, win));
            }

            var progress = SaveManager.Instance.Progress;
            return new RunMetrics
            {
                Ascension = run.Ascension,
                TotalPlaytime = progress.TotalPlaytime,
                TotalWinRate = progress.Wins / (float)progress.NumberOfRuns,
                NumReloads = run.NumReloads,
                BuildId = ReleaseInfoManager.Instance.ReleaseInfo?.Version ?? "NON-RELEASE-VERSION",
                BuildType = PlatformUtil.GetPlatformBranch().ToName(),
                PlayerId = progress.UniqueId,
                Character = localPlayer.CharacterId ?? ModelId.none,
                NumPlayers = run.Players.Count,
                Team = run.Players.Count > 1
                    ? run.Players
                        .Select<SerializablePlayer, ModelId>(p => p.CharacterId ?? ModelId.none)
                        .ToList()
                    : [],
                Win = isVictory,
                FloorReached = list1.Count,
                KilledByEncounter = modelId ?? ModelId.none,
                Deck = localPlayer.Deck.Select<SerializableCard, ModelId>(c => c.Id ?? ModelId.none),
                Relics = localPlayer.Relics.Select<SerializableRelic, ModelId>(r => r.Id ?? ModelId.none),
                RunPlaytime = run.WinTime > 0L ? run.WinTime : run.RunTime,
                Encounters = list2,
                CardChoices = list3,
                EventChoices = eventChoiceMetricList,
                AncientChoices = list4,
                ActWins = actWinMetricList,
                CampfireUpgrades = list1
                    .Where(
                        (e => e.MapPointType == MapPointType.RestSite))
                    .SelectMany(e =>
                        e.GetEntry(localPlayerId).UpgradedCards)
                    .Select<ModelId, string>(c => c.Entry).ToList(),
                RelicBuys = list1
                    .SelectMany(e =>
                        e.GetEntry(localPlayerId).BoughtRelics)
                    .Select<ModelId, string>(r => r.Entry).ToList(),
                PotionBuys = list1
                    .SelectMany(e =>
                        e.GetEntry(localPlayerId).BoughtPotions)
                    .Select<ModelId, string>(p => p.Entry).ToList(),
                ColorlessBuys = list1
                    .SelectMany(e =>
                        e.GetEntry(localPlayerId).BoughtColorless)
                    .Select<ModelId, string>(c => c.Entry).ToList(),
                PotionDiscards = list1
                    .SelectMany(e =>
                        e.GetEntry(localPlayerId).PotionDiscarded)
                    .Select<ModelId, string>(p => p.Entry).ToList()
            };
        }
        finally
        {
            LocManager.Instance.StopOverridingLanguageAsEnglish();
        }
    }
}