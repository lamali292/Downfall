using Downfall.Code.Nodes;
using Downfall.Code.Saves;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Core.Collector;

public static class CollectiblesModel
{
    
    public static List<CardModel> GetCollectibles(Player player) => 
        DownfallSaveManager.GetPlayerData(player.NetId).CollectorDeck.Select(CardModel.FromSerializable).ToList();
    

    public static void AddCollectible(Player player, CardModel card)
    {
        var data = DownfallSaveManager.GetPlayerData(player.NetId);
        data.CollectorDeck.Add(card.ToSerializable());
        
        NTopBarCollectorButton.RefreshButton();
    }
}