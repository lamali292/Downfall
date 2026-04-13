using BaseLib.Utils;
using Downfall.Code.Nodes;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Core.Collector;

public static class EssenceModel
{
    private static readonly SavedSpireField<RelicModel, int> Essence = new(
        () => 0,
        "Downfall_Collector_Essence"
    );

    public static int GetEssence(Player player) => Essence.Get(StartingRelic(player));

    public static void AddEssence(Player player, int amount)
    {
        var relic = StartingRelic(player);
        Essence.Set(relic, Essence.Get(relic) + amount);
        NTopBarEssenceDisplay.RefreshDisplay();
    }

    public static bool SpendEssence(Player player, int amount)
    {
        var relic = StartingRelic(player);
        if (Essence.Get(relic) < amount) return false;
        Essence.Set(relic, Essence.Get(relic) - amount);
        NTopBarEssenceDisplay.RefreshDisplay();
        return true;
    }

    public static bool CanAfford(Player player, int amount) => Essence.Get(StartingRelic(player)) >= amount;

    private static RelicModel StartingRelic(Player player)
    {
        var startingRelics = player.Character.StartingRelics;
        if (startingRelics.Count == 0)
            throw new Exception("No starting relic defined for character.");
        var relicId = startingRelics[0].Id;
        var actualRelic = player.GetRelicById(relicId);
        return actualRelic ?? throw new Exception("No relic with id " + relicId);
    }
}