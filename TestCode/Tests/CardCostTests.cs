using Downfall.DownfallCode.Extensions;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Downfall.TestCode;

/// Pins down the answers of the shared card-cost module on real cards: an X-energy card (Whirlwind),
/// an X-star card (Stardust), a normal card (Strike) and a negative-cost unplayable (Ascender's Bane).
public class CardCostTests
{
    [CardTest]
    public async Task XEnergyCardHasNoNumericCostAndIsNeverFree(TestContext ctx)
    {
        var card = await ctx.AddCardToHand<Whirlwind>();

        Assert.IsTrue(card.IsXEnergy, "Whirlwind costs X energy.");
        Assert.IsTrue(!card.HasNumericCost, "An X-energy card has no numeric cost.");
        Assert.IsTrue(!card.IsFreeNow, "An X-energy card is never free, even though its base cost is stored as 0.");
        Assert.AreEqual(0, card.PrintedCost, "X cards report a printed cost of 0.");
    }

    [CardTest]
    public async Task XEnergyCardEffectiveCostIsCurrentEnergy(TestContext ctx)
    {
        var card = await ctx.AddCardToHand<Whirlwind>();
        ctx.Player.PlayerCombatState!.Energy = 2;

        Assert.AreEqual(2, card.EffectiveCost, "An X-energy card pays all the energy the player has.");
    }

    [CardTest]
    public async Task XStarCardHasNoNumericCostAndIsNotXEnergy(TestContext ctx)
    {
        var card = await ctx.AddCardToHand<Stardust>();

        Assert.IsTrue(!card.HasNumericCost, "An X-star card has no numeric cost.");
        Assert.IsTrue(!card.IsXEnergy, "An X-star card does not cost X energy.");
        Assert.IsTrue(!card.IsFreeNow, "An X-star card is never free.");
    }

    [CardTest]
    public async Task NumericCardSeparatesPrintedFromCurrentCost(TestContext ctx)
    {
        var card = await ctx.AddCardToHand<StrikeIronclad>();

        Assert.IsTrue(card.HasNumericCost, "Strike has a numeric cost.");
        Assert.AreEqual(1, card.PrintedCost, "Strike prints 1.");
        Assert.AreEqual(1, card.EffectiveCost, "Strike currently costs 1.");
        Assert.IsTrue(!card.IsFreeNow, "A 1-cost card is not free.");

        card.EnergyCost.SetThisTurn(0);

        Assert.AreEqual(1, card.PrintedCost, "Printed cost ignores modifiers.");
        Assert.AreEqual(0, card.EffectiveCost, "Effective cost follows the modifier.");
        Assert.IsTrue(card.IsFreeNow, "A card reduced to 0 is free right now.");
    }

    [CardTest]
    public async Task NegativeCostCardIsNotFreeAndKeepsModifiedCost(TestContext ctx)
    {
        var card = await ctx.AddCardToHand<AscendersBane>();

        Assert.IsTrue(card.ModifiedCost < 0, "An unplayable curse has a negative modified cost.");
        Assert.AreEqual(0, card.EffectiveCost, "The amount to pay never goes below zero.");
        Assert.IsTrue(!card.IsFreeNow, "A negative-cost card must not look free.");
    }
}
