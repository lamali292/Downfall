namespace Collector.CollectorCode.Core;

/// <summary>
/// How a card's cost is split between Energy and Reserve. <see cref="ReserveSpent"/> is the Reserve taken;
/// <see cref="ConvertedToEnergy"/> is true for X-energy cards, where that Reserve is moved into Energy
/// (so the game's own X plumbing sees Energy + Reserve) instead of being paid towards a fixed cost.
/// </summary>
public readonly record struct ReservePayment(int ReserveSpent, bool ConvertedToEnergy);

/// <summary>
/// Reserve pay and affordability decisions over plain numbers, so they can be tested without the game.
/// The Harmony patches read the numbers off the card and player, call in here, and apply the result.
/// </summary>
public static class ReservePaymentRules
{
    /// <summary>
    /// Can the player pay <paramref name="cost"/>? Reserve-only cards need enough Reserve alone; every other
    /// card can combine Energy and Reserve.
    /// </summary>
    public static bool CanAfford(int energy, int reserve, int cost, bool reserveOnly)
    {
        return reserveOnly ? reserve >= cost : energy + reserve >= cost;
    }

    /// <summary>
    /// How much Reserve a card takes when played.
    /// X-energy cards convert all Reserve (checked first, even for reserve-only cards).
    /// Reserve-only cards pay the full cost from Reserve, or nothing if they cannot afford it.
    /// Other cards pay from Energy first and cover only the missing part from Reserve.
    /// </summary>
    public static ReservePayment Pay(int energy, int reserve, int cost, bool costsX, bool reserveOnly)
    {
        if (costsX)
            return new ReservePayment(Math.Max(0, reserve), true);

        if (reserveOnly)
            return new ReservePayment(CanAfford(energy, reserve, cost, true) ? cost : 0, false);

        if (energy >= cost)
            return new ReservePayment(0, false);

        return new ReservePayment(Math.Min(cost - energy, reserve), false);
    }
}
