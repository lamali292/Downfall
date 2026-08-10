using System.Reflection;
using System.Text.Json;
using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.Debug;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Platform;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs.History;
using MegaCrit.Sts2.Core.Runs.Metrics;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace Downfall.DownfallCode.Data;

public static class DownfallMetrics
{
    private static readonly RunMetricsUploader<RunMetrics> Uploader = new(
        new MetricsUploaderConfig
        {
            ModName = "Downfall",
            EndpointUrl = "https://wxememsxgrgrfvntulgr.supabase.co/rest/v1/runs",
            ApiKey = "sb_publishable_XJRuWuyy0fJwKVFUQ8L3Dw_JrokGm_i",
            ModVersionProvider = DownfallMainFile.GetDownfallVersion,
            IsOwnCharacter = MetricsPredicates.CharacterOfType<DownfallCharacterModel>(),
            AllowedAssemblies = new HashSet<Assembly>
            {
                typeof(DownfallCharacterModel).Assembly,
                typeof(CharacterModel).Assembly,
            },
            Logger = DownfallMainFile.Logger,
        },
        buildPayload: GetRunMetrics,
        serialize: m => JsonSerializer.Serialize(m, MetricsSerializerContext.Default.RunMetrics));

    internal static void OnMetricsUpload(SerializableRun run, bool isVictory, ulong localPlayerId)
        => Uploader.Upload(run, isVictory, localPlayerId);

    private static RunMetrics GetRunMetrics(
        SerializableRun run,
        bool isVictory,
        ulong localPlayerId)
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
        var encounters = list1
            .Where(e =>
                e.Rooms.Last().RoomType.IsCombatRoom())
            .Select(e =>
                new EncounterMetric((e.Rooms.Last().ModelId ?? ModelId.none).Entry,
                    int.Min(e.GetEntry(localPlayerId).DamageTaken, localPlayer.MaxHp),
                    e.Rooms.Last().TurnsTaken + 1)).ToList();
        var cardChoices = list1
            .Where(
                (Func<MapPointHistoryEntry, bool>)(e => e.GetEntry(localPlayerId).CardChoices.Count > 0))
            .Select(
                (Func<MapPointHistoryEntry, CardChoiceMetric>)(e =>
                    new CardChoiceMetric(e.GetEntry(localPlayerId).CardChoices))).ToList();
        var ancientChoices = list1
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
            Encounters = encounters,
            CardChoices = cardChoices,
            EventChoices = eventChoiceMetricList,
            AncientChoices = ancientChoices,
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
}