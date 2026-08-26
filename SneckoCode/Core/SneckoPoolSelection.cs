using BaseLib.Extensions;
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
    public static void RunActEntry(IRunState runstate)   // no await left here → not async
    {
        var sneckos = runstate.Players.Where(p => p.Character is Snecko).ToList();

        // PHASE 1 — reserve ids synchronously (unchanged, keeps MP in sync)
        var plans = new List<(Player player, SneckoChoice[] relics, uint[] choiceIds)>();
        foreach (var player in sneckos)
        {
            var relics = new SneckoChoice[3];
            var ids    = new uint[3];
            for (var i = 0; i < 3; i++)
            {
                relics[i] = (SneckoChoice)ModelDb.Relic<SneckoChoice>().ToMutable();
                ids[i]    = RunManager.Instance.PlayerChoiceSynchronizer.ReserveChoiceId(player);
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
        catch (OperationCanceledException) { }
        catch (Exception e) { SneckoMainFile.Logger.Error($"[Snecko] deferred selection failed: {e}"); }
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
            var left  = six[i * 2];
            var right = six[i * 2 + 1];
            var index = await SyncOneChoice(player, left, right, choiceIds[i]);
            relics[i].InitCharacter(index == 0 ? left : right);
            await RelicCmd.Obtain(relics[i], player);   // obtain right after this pick
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