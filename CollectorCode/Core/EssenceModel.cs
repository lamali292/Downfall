using BaseLib.Utils;
using Collector.CollectorCode.Vfx;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Collector.CollectorCode.Core;

public static class EssenceModel
{
    public static SavedSpireField<Player, int> Essence = new(() => 0, "CollectorEssence");

    public static int GetEssence(Player player)
    {
        return Essence.Get(player);
    }

    public static void AddEssence(Player player, int amount)
    {
        Essence.Set(player, Essence.Get(player) + amount);
        NTopBarEssenceDisplay.RefreshDisplay();
    }

    public static void ClearEssence(Player player)
    {
        Essence.Set(player, 0);
    }

    public static bool SpendEssence(Player player, int amount)
    {
        var essence = Essence.Get(player);
        if (essence < amount) return false;
        essence -= amount;
        Essence.Set(player, essence);
        NTopBarEssenceDisplay.RefreshDisplay();
        return true;
    }

    public static bool CanAfford(Player player, int amount)
    {
        return GetEssence(player) >= amount;
    }
}