using BaseLib.Utils;
using Downfall.Code.Nodes;
using Downfall.Code.Saves;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Core.Collector;

public static class EssenceModel
{
    // Reach directly into the SaveManager using the player's NetId
    public static int GetEssence(Player player) => 
        DownfallSaveManager.GetPlayerData(player).Essence;

    public static void AddEssence(Player player, int amount)
    {
        DownfallSaveManager.GetPlayerData(player).Essence += amount;
        NTopBarEssenceDisplay.RefreshDisplay();
    }
    
    public static void ClearEssence(Player player)
    {
        DownfallSaveManager.GetPlayerData(player).Essence = 0;
    }

    public static bool SpendEssence(Player player, int amount)
    {
        var data = DownfallSaveManager.GetPlayerData(player);
        if (data.Essence < amount) return false;
        
        data.Essence -= amount;
        NTopBarEssenceDisplay.RefreshDisplay();
        return true;
    }

    public static bool CanAfford(Player player, int amount) => 
        GetEssence(player) >= amount;
}