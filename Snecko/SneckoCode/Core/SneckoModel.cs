using BaseLib.Abstracts;
using Downfall.DownfallCode.Saves;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using Snecko.SneckoCode.Cards;

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
    
   
    public override async Task AfterActEntered()
    {
        var state = RunManager.Instance.State;
        if (state is not { CurrentActIndex: 0 }) return;
        foreach (var snecko in state.Players.Where(e => e.Character is Snecko))
        {
            var a = ModelDb.AllCharacterCardPools
                .Where(e => e != snecko.Character.CardPool)
                .TakeRandom(3, state.Rng.UpFront).ToList();
            SetSneckoPools(snecko, a);
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
}