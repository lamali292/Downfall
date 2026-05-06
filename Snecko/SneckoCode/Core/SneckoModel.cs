using BaseLib.Abstracts;
using Downfall.DownfallCode.Saves;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Screens.Overlays;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using Snecko.SneckoCode.Cards;
using Snecko.SneckoCode.Vfx;

namespace Snecko.SneckoCode.Core;

public class SneckoModel(): CustomSingletonModel(true, true)
{
   public override async Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? source)
    {
        if (oldPileType == PileType.None && card.Pile?.Type == PileType.Deck && card is SneckoCardModel { Gift: { } gift })
        {
            await SneckoCmd.GetGift(card.Owner, gift);
        }
        
    }
    
  
    private static void SetSneckoPools(Player player, IEnumerable<CardPoolModel> pools)
    {
        var pool = DownfallSaveManager.GetPlayerData(player).SneckoPools;
        pool.Clear();
        pool.AddRange(pools.Select(e => e.Id));
    }

    private static IEnumerable<CardPoolModel> GetSneckoPools(Player player)
    {
        return DownfallSaveManager.GetPlayerData(player).SneckoPools.Select(ModelDb.GetById<CardPoolModel>);
    }

    public static IEnumerable<CardModel> GetSneckoCards(Player player)
        => GetSneckoPools(player).SelectMany(e => e.AllCards);
    
    
    public override async Task AfterActEntered()
    {
        var state = RunManager.Instance.State;
        if (state is not { CurrentActIndex: 0 }) return;

        foreach (var snecko in state.Players.Where(e => e.Character is Snecko))
            await DoOffclassSelection(snecko, state);
    }

    private static async Task DoOffclassSelection(Player snecko, IRunState state)
    {
        var sixCharacters = GetSixCharacters(snecko, state);

        var selectScene = await TryShowSelectionScreen(snecko);

        var chosenCharacters = await SyncSelections(snecko, sixCharacters, selectScene);

        TearDownSelectionScreen(selectScene);

        SetSneckoPools(snecko, chosenCharacters.Select(c => c.CardPool).ToList());
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
        NSneckoCharacterSelect? selectScene)
    {
        var chosen = new List<CharacterModel>();
        for (var i = 0; i < 3; i++)
        {
            var left  = sixCharacters[i * 2];
            var right = sixCharacters[i * 2 + 1];
            var index = await SyncOneChoice(snecko, left, right, selectScene);
            chosen.Add(index == 0 ? left : right);
        }
        return chosen;
    }

    private static async Task<int> SyncOneChoice(
        Player snecko,
        CharacterModel left,
        CharacterModel right,
        NSneckoCharacterSelect? selectScene)
    {
        var choiceId = RunManager.Instance.PlayerChoiceSynchronizer.ReserveChoiceId(snecko);
        int chosenIndex;

        if (LocalContext.IsMe(snecko))
        {
            chosenIndex = await GetLocalChoice(left, right, selectScene);
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
        CharacterModel right,
        NSneckoCharacterSelect? selectScene)
    {
        if (selectScene == null) return 0;
        try
        {
            return await selectScene.SelectOne(left, right);
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