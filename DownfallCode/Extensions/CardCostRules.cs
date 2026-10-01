namespace Downfall.DownfallCode.Extensions;

/// <summary>
/// The cost decisions, expressed over plain values so they can be tested without the game loaded.
/// Game code should not call this directly: use the <c>CardCostExtensions</c> properties on
/// <c>CardModel</c>, which read the values off the card and feed them in here.
/// </summary>
public static class CardCostRules
{
    /// <summary>
    /// A card has a numeric cost unless it is X-energy or X-star. X cards have no fixed number to
    /// compare, sort or randomize, so effects that work on "the cost" must skip them.
    /// </summary>
    public static bool HasNumericCost(bool costsXEnergy, bool costsXStar)
    {
        return !costsXEnergy && !costsXStar;
    }

    /// <summary>
    /// What the player pays right now. X-energy cards pay everything they have; other cards pay their
    /// modified cost, never below zero (the game's "amount to spend" rule).
    /// </summary>
    public static int EffectiveCost(bool costsXEnergy, int energyAvailable, int modifiedCost)
    {
        return costsXEnergy ? energyAvailable : Math.Max(0, modifiedCost);
    }
}
