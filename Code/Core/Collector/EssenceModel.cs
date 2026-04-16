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
        DownfallSaveManager.GetPlayerData(player.NetId).Essence;

    public static void AddEssence(Player player, int amount)
    {
        var data = DownfallSaveManager.GetPlayerData(player.NetId);
        data.Essence += amount;
        NTopBarEssenceDisplay.RefreshDisplay();
    }

    public static bool SpendEssence(Player player, int amount)
    {
        var data = DownfallSaveManager.GetPlayerData(player.NetId);
        if (data.Essence < amount) return false;
        
        data.Essence -= amount;
        NTopBarEssenceDisplay.RefreshDisplay();
        return true;
    }

    public static bool CanAfford(Player player, int amount) => 
        GetEssence(player) >= amount;
}