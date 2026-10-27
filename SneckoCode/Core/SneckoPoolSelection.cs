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
using Snecko.SneckoCode.Interfaces;
using Snecko.SneckoCode.Relics;

namespace Snecko.SneckoCode.Core;

public static class SneckoPoolSelection
{
    internal const int RoundCount = 3;

    // In-memory gate against SneckoModel.AfterRoomEntered firing more than once *in the same
    // session* for act 1 floor 1 - observed when another mod re-triggers the same room-entered hook.
    // Keyed by the run itself (not just a plain bool) so it resets naturally for the next run without
    // needing an explicit AfterRunEnd. This alone is NOT enough: save-quit-and-reload deserializes a
    // brand new IRunState (a different key), which would defeat this gate even though the player
    // already has their SneckoChoice relics from before the save - see IsDoneSelecting below, which
    // is what actually survives a reload. The lock makes the check-and-set atomic - belt and
    // suspenders in case some future caller invokes this from other than the main thread; on the main
    // thread alone a plain check would already be safe since nothing here awaits between the read and
    // the write.
    private static readonly Lock Gate = new();
    private static readonly SpireField<IRunState, bool> HasRunActEntry = new(_ => false);

    // How much of the selection this player has already covered, per ISneckoPoolSupplier.ActEntryWeight
    // (a normal SneckoChoice counts for 1, Prismatic Snecko counts for the whole RoundCount on its
    // own) - so neither this method nor RunPlayer need to know about any specific supplier type.
    private static bool IsDoneSelecting(Player player)
    {
        return player.Relics.OfType<ISneckoPoolSupplier>().Sum(s => s.ActEntryWeight) >= RoundCount;
    }

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
        // Players who are already done selecting (from before a save/reload, or from this same
        // method somehow already having granted them) are skipped rather than run through the picker
        // again.
        var sneckos = runstate.Players
            .Where(p => p.Character is Snecko)
            .Where(p => !IsDoneSelecting(p))
            .ToList();
        if (sneckos.Count == 0) return;

        // PHASE 1 — reserve ids synchronously (unchanged, keeps MP in sync)
        var plans = new List<(Player player, SneckoChoice[] relics, uint[] choiceIds)>();
        foreach (var player in sneckos)
        {
            var relics = new SneckoChoice[RoundCount];
            var ids = new uint[RoundCount];
            for (var i = 0; i < RoundCount; i++)
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

        for (var i = 0; i < RoundCount; i++)
        {
            var left = six[i * 2];
            var right = six[i * 2 + 1];

            // Each option carries its own card (what the screen shows) and how to obtain it if
            // picked - no index/type checking needed to tell them apart afterwards. Prismatic Snecko
            // is only offered on the very first round - once the player has passed on it, later
            // rounds go back to a plain 2-way character choice.
            List<(CardModel Card, Func<Task> Obtain)> options =
            [
                (CharacterCard.Create(left), () => ObtainCharacterChoice(relics[i], left, player)),
                (CharacterCard.Create(right), () => ObtainCharacterChoice(relics[i], right, player))
            ];
            if (i == 0)
                options.Add((PrismaticSneckoCard.Create(),
                    () => RelicCmd.Obtain(ModelDb.Relic<PrismaticSnecko>().ToMutable(), player)));

            var chosenIndex = await SyncOneChoice(player, options.Select(o => o.Card).ToList(), choiceIds[i]);
            await options[chosenIndex].Obtain();

            // Stop as soon as the player has covered every character - whether via RoundCount
            // individual SneckoChoice picks or the one Prismatic Snecko - rather than hardcoding
            // "did they pick the last option".
            if (IsDoneSelecting(player)) return;
        }
    }

    private static Task ObtainCharacterChoice(SneckoChoice relic, CharacterModel character, Player player)
    {
        relic.InitCharacter(character);
        return RelicCmd.Obtain(relic, player);
    }

    private static async Task<int> SyncOneChoice(Player snecko, IReadOnlyList<CardModel> options, uint choiceId)
    {
        int chosenIndex;
        if (LocalContext.IsMe(snecko))
        {
            chosenIndex = await GetLocalChoice(options);
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

    private static async Task<int> GetLocalChoice(IReadOnlyList<CardModel> options)
    {
        var screen = NChooseACardSelectionScreen.ShowScreen(options, false);
        if (screen == null) return 0;
        var result = (await screen.CardsSelected()).ToList();
        for (var i = 0; i < options.Count; i++)
            if (result.Contains(options[i]))
                return i;
        return 0;
    }
}