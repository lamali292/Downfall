using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Screens.CardSelection;
using MegaCrit.Sts2.Core.Runs;
using Snecko.SneckoCode.Cards;
using Snecko.SneckoCode.Relics;

namespace Snecko.SneckoCode.Core;

public static class SneckoPoolSelection
{
    private const int RoundCount = 3;

    // In-memory gate against SneckoModel.AfterRoomEntered firing more than once *in the same
    // session* for act 1 floor 1 - observed when another mod re-triggers the same room-entered hook.
    // Keyed by the run itself (not just a plain bool) so it resets naturally for the next run without
    // needing an explicit AfterRunEnd. This alone is NOT enough: save-quit-and-reload deserializes a
    // brand new IRunState (a different key), which would defeat this gate even though the player
    // already has their SneckoChoice relics from before the save - see the per-player relic count
    // filter below, which is what actually survives a reload. The lock makes the check-and-set atomic
    // - belt and suspenders in case some future caller invokes this from other than the main thread;
    // on the main thread alone a plain check would already be safe since nothing here awaits between
    // the read and the write.
    private static readonly Lock Gate = new();
    private static readonly SpireField<IRunState, bool> HasRunActEntry = new(_ => false);

    public static void RunActEntry(IRunState runstate) // no await left here → not async
    {
        lock (Gate)
        {
            if (HasRunActEntry[runstate]) return;
            HasRunActEntry[runstate] = true;
        }

        // This runs once per LOCAL client for every Snecko player in the run (not just the local
        // player) so every client reserves the same choice ids in the same order - each client's own
        // HasRunActEntry gate above is process-local and doesn't need to be synced for that to hold.
        // Players who already hold a full set of SneckoChoice relics (from before a save/reload, or
        // from this same method somehow already having granted them) are skipped rather than run
        // through the picker again.
        var sneckos = runstate.Players
            .Where(p => p.Character is Snecko)
            .Where(p => p.Relics.OfType<SneckoChoice>().Count() < RoundCount)
            .ToList();
        if (sneckos.Count == 0) return;

        // PHASE 1 — reserve ids synchronously (unchanged, keeps MP in sync)
        var plans = new List<(Player player, SneckoChoice[] relics, uint[] choiceIds)>();
        foreach (var player in sneckos)
        {
            var relics = new SneckoChoice[3];
            var ids = new uint[3];
            for (var i = 0; i < 3; i++)
            {
                relics[i] = (SneckoChoice)ModelDb.Relic<SneckoChoice>().ToMutable();
                ids[i] = RunManager.Instance.PlayerChoiceSynchronizer.ReserveChoiceId(player);
            }

            plans.Add((player, relics, ids));
        }

        // PHASE 2 — void-returning lambda; discard the Task so it binds to Action, not Func<Task>.
        Callable.From(() => { _ = RunPicks(plans); }).CallDeferred();
    }


    private static async Task RunPicks(
        List<(Player player, SneckoChoice[] relics, uint[] choiceIds)> plans)
    {
        try
        {
            await Task.WhenAll(plans.Select(RunPlayer));
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception e)
        {
            SneckoMainFile.Logger.Error($"[Snecko] deferred selection failed: {e}");
        }
    }

    private static async Task RunPlayer(
        (Player player, SneckoChoice[] relics, uint[] choiceIds) plan)
    {
        var (player, relics, choiceIds) = plan;

        var six = ModelDb.AllCharacters
            .Where(c => c != player.Character)
            .TakeRandom(6, player.RunState.Rng.UpFront)
            .ToList();

        for (var i = 0; i < 3; i++)
        {
            var left = six[i * 2];
            var right = six[i * 2 + 1];
            var index = await SyncOneChoice(player, left, right, choiceIds[i]);
            relics[i].InitCharacter(index == 0 ? left : right);
            await RelicCmd.Obtain(relics[i], player); // obtain right after this pick
        }
    }

    private static async Task<int> SyncOneChoice(
        Player snecko, CharacterModel left, CharacterModel right, uint choiceId)
    {
        int chosenIndex;
        if (LocalContext.IsMe(snecko))
        {
            chosenIndex = await GetLocalChoice(left, right);
            RunManager.Instance.PlayerChoiceSynchronizer.SyncLocalChoice(
                snecko, choiceId, PlayerChoiceResult.FromIndex(chosenIndex));
        }
        else
        {
            chosenIndex = (await RunManager.Instance.PlayerChoiceSynchronizer
                .WaitForRemoteChoice(snecko, choiceId)).AsIndex();
        }

        return chosenIndex;
    }

    private static async Task<int> GetLocalChoice(CharacterModel left, CharacterModel right)
    {
        var card1 = CharacterCard.Create(left);
        var card2 = CharacterCard.Create(right);
        var screen = NChooseACardSelectionScreen.ShowScreen([card1, card2], false);
        if (screen == null) return 0;
        var result = (await screen.CardsSelected()).ToList();
        return result.Contains(card1) ? 0 : 1;
    }
}