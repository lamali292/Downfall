using Collector.CollectorCode.Cards.Uncommon;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Extensions;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Downfall.TestCode;

public class CollectorTests
{
    // Regression guard for a previously-missing feature: X-cost cards only spent Energy and never
    // touched Reserve, so Collector's Reserve resource did nothing to boost their effect. CardModel.
    // SpendResources() is the only place that actually deducts Energy/sets CapturedXValue (CardCmd.
    // AutoPlay, which TestContext.PlayCard uses, plays cards "for free" and never calls it), so this
    // calls it directly rather than going through ctx.PlayCard.
    [CardTest]
    public async Task XCostCardSpendsEnergyAndReserve(TestContext ctx)
    {
        var card = await ctx.AddCardToHand<Whirlwind>();
        ctx.Player.PlayerCombatState!.Energy = 2;
        await CollectorCmd.GainReserve(ctx.Player, 3);

        var (energySpent, _) = await card.SpendResources();

        Assert.AreEqual(5, energySpent, "X-cost card should report Energy + Reserve as spent.");
        Assert.AreEqual(5, card.EnergyCost.CapturedXValue, "X value should be Energy + Reserve.");
        Assert.AreEqual(0, ctx.Player.PlayerCombatState.Energy, "All Energy should be spent.");
        Assert.AreEqual(0, ctx.Player.PlayerCombatState.Reserve, "All Reserve should be spent.");
    }

    [CardTest]
    public async Task XCostCardWithNoReserveOnlySpendsEnergy(TestContext ctx)
    {
        var card = await ctx.AddCardToHand<Whirlwind>();
        ctx.Player.PlayerCombatState!.Energy = 3;

        var (energySpent, _) = await card.SpendResources();

        Assert.AreEqual(3, energySpent, "With no Reserve, X-cost card should only spend Energy.");
        Assert.AreEqual(3, card.EnergyCost.CapturedXValue, "X value should equal Energy when Reserve is empty.");
        Assert.AreEqual(0, ctx.Player.PlayerCombatState.Energy, "All Energy should be spent.");
        Assert.AreEqual(0, ctx.Player.PlayerCombatState.Reserve, "Reserve should remain empty.");
    }

    // Regression guard for the existing (non-X-cost) behavior: Reserve should still only cover the
    // Energy deficit, unaffected by the X-cost handling added above.
    [CardTest(typeof(Collector.CollectorCode.Core.Collector))]
    public async Task NormalCostCardStillUsesReserveOnlyToCoverDeficit(TestContext ctx)
    {
        var card = await ctx.AddCardToHand<BidingBlast>();
        ctx.Player.PlayerCombatState!.Energy = 0;
        await CollectorCmd.GainReserve(ctx.Player, 5);

        var (energySpent, _) = await card.SpendResources();

        Assert.AreEqual(1, energySpent, "BidingBlast costs 1.");
        Assert.AreEqual(0, ctx.Player.PlayerCombatState.Energy, "Energy was already empty.");
        Assert.AreEqual(4, ctx.Player.PlayerCombatState.Reserve, "Only the 1-cost deficit should be covered by Reserve.");
    }
}
