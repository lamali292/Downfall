using Downfall.DownfallCode.Cards;
using Godot;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Screens.CardSelection;
using MegaCrit.Sts2.Core.Nodes.Screens.Overlays;
using MegaCrit.Sts2.Core.Runs;
using Snecko.SneckoCode.Vfx;

namespace Snecko.SneckoCode.Core;

public static class SneckoPoolSelection
{
    public static async Task<List<CardPoolModel>> DoOffclassSelection(
        Player snecko, IRunState state, uint[] choiceIds)
    {
        var sixCharacters = GetSixCharacters(snecko, state);
        //var selectScene = await TryShowSelectionScreen(snecko);
        var chosenCharacters = await SyncSelections(snecko, sixCharacters, choiceIds);
        //TearDownSelectionScreen(selectScene);
        return chosenCharacters.Select(c => c.CardPool).ToList();
    }

    private static List<CharacterModel> GetSixCharacters(Player snecko, IRunState state)
    {
        return ModelDb.AllCharacters
            .Where(e => e != snecko.Character)
            .TakeRandom(6, state.Rng.UpFront)
            .ToList();
    }

    private static async Task<NSneckoCharacterSelect?> TryShowSelectionScreen(Player snecko)
    {
        if (!LocalContext.IsMe(snecko) || NOverlayStack.Instance == null || NGame.Instance == null)
            return null;

        var selectScene = new NSneckoCharacterSelect();
        NOverlayStack.Instance.Push(selectScene);
        await NGame.Instance.ToSignal(NGame.Instance.GetTree(), SceneTree.SignalName.ProcessFrame);
        return selectScene;
    }

    private static async Task<List<CharacterModel>> SyncSelections(
        Player snecko,
        List<CharacterModel> sixCharacters,
        uint[] choiceIds)
    {
        var chosen = new List<CharacterModel>();
        for (var i = 0; i < 3; i++)
        {
            var left = sixCharacters[i * 2];
            var right = sixCharacters[i * 2 + 1];
            var index = await SyncOneChoice(snecko, left, right, choiceIds[i]);
            chosen.Add(index == 0 ? left : right);
        }

        return chosen;
    }

    private static async Task<int> SyncOneChoice(
        Player snecko,
        CharacterModel left,
        CharacterModel right,
        uint choiceId)
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

    private static async Task<int> GetLocalChoice(
        CharacterModel left,
        CharacterModel right)
    {
        //if (selectScene == null) return 0;
        try
        {
            var card1 = CharacterCard.Create(left);
            var card2 = CharacterCard.Create(right);
            //CardSelectCmd.FromChooseACardScreen()
            
            var screen = NChooseACardSelectionScreen.ShowScreen([card1, card2], false);
            if (screen == null) return 0;
            var result = (await screen.CardsSelected()).ToList();
            return (result.Contains(card1)) ? 0 : 1;
            //return await selectScene.SelectOne(left, right);
        }
        catch (Exception e)
        {
            GD.PrintErr($"SelectOne failed: {e.Message}");
            return 0;
        }
    }

    private static void TearDownSelectionScreen(NSneckoCharacterSelect? selectScene)
    {
        if (selectScene == null) return;
        NOverlayStack.Instance?.Remove(selectScene);
        selectScene.QueueFree();
    }
}