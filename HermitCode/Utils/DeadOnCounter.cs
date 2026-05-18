using MegaCrit.Sts2.Core.Entities.Players;

namespace Hermit.HermitCode.Utils;

public static class DeadOnCounter
{
    private static readonly Dictionary<Player, int> CounterValues = [];
    private static readonly Dictionary<Player, bool> IsLastCardDeadOn = [];

    public static int GetCounter(Player player)
    {
        return CounterValues.GetValueOrDefault(player, 0);
    }

    public static bool GetIsLastCardDeadOn(Player player)
    {
        return IsLastCardDeadOn.GetValueOrDefault(player, false);
    }

    public static void IncreaseCounter(Player player)
    {
        CounterValues[player] = GetCounter(player) + 1;
    }

    public static void SetIsLastCardDeadOn(Player player, bool value)
    {
        IsLastCardDeadOn[player] = value;
    }
}