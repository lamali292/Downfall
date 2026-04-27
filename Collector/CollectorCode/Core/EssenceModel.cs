using Collector.CollectorCode.Vfx;
using Downfall.DownfallCode.Saves;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Collector.CollectorCode.Core;

public static class EssenceModel
{
    // Reach directly into the SaveManager using the player's NetId
    public static int GetEssence(Player player)
    {
        return DownfallSaveManager.GetPlayerData(player).Essence;
    }

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

    public static bool CanAfford(Player player, int amount)
    {
        return GetEssence(player) >= amount;
    }
}