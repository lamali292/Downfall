using Awakened.AwakenedCode.Cards.Basic;
using Awakened.AwakenedCode.Cards.Common;

namespace Downfall.TestCode;

public class AwakenedTests
{
    private static int ZeroCostCardsInHand(TestContext ctx) =>
        ctx.Player.Hand.Count(c => c.EnergyCost.GetAmountToSpend() == 0 && !c.EnergyCost.CostsX);

    // Clutch doesn't draw (it puts a random 0-cost card from the draw pile into hand, per its own
    // wording), so it shouldn't interact with draw hooks/relics like Fiddle at all. The pick is
    // random and Awakened's own starting deck already has a 0-cost card (Hymn), so this asserts on
    // the count rather than a specific seeded instance - checking for one exact card would be flaky.
    [CardTest(typeof(Awakened.AwakenedCode.Core.Awakened))]
    public async Task ClutchPutsZeroCostCardIntoHand(TestContext ctx)
    {
        await ctx.AddCardToTopOfDraw<Hymn>();
        var clutch = await ctx.AddCardToHand<Clutch>();
        var before = ZeroCostCardsInHand(ctx);

        await ctx.PlayCard(clutch, ctx.Combat.HittableEnemies.First());

        Assert.AreEqual(before + 1, ZeroCostCardsInHand(ctx), "Clutch should put exactly one 0-cost card into hand.");
    }
}
