using Awakened.AwakenedCode.Cards.Basic;
using Awakened.AwakenedCode.Cards.Common;
using MegaCrit.Sts2.Core.AutoSlay;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Relics;

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

    // Regression guard: a card that Snecko Eye randomized to a non-zero cost must keep that cost
    // (SetThisCombat persists on the CardModel instance regardless of pile) after being discarded
    // and reshuffled back into the draw pile, and Clutch must respect it - reported as "Clutch
    // brought me this 2-cost Blunder Guard" after a Snecko Eye reshuffle.
    [CardTest(typeof(Awakened.AwakenedCode.Core.Awakened))]
    public async Task ClutchDoesNotPickCardsRandomizedNonZeroBySneckoEye(TestContext ctx)
    {
        var sneckoEye = await RelicCmd.Obtain<SneckoEye>(ctx.Player);
        sneckoEye.SetTestEnergyCostOverride(2);

        var hymn = await ctx.AddCardToTopOfDraw<Hymn>();
        var drawnCard = await CardPileCmd.Draw(new BlockingPlayerChoiceContext(), ctx.Player);
        AutoSlayLog.Info($"[AwakenedTests] drawn={drawnCard == hymn} cost={hymn.EnergyCost.GetAmountToSpend()}");
        Assert.AreEqual(hymn, drawnCard, "Sanity check: should have drawn the seeded Hymn.");
        Assert.AreEqual(2, hymn.EnergyCost.GetAmountToSpend(), "Snecko Eye should have randomized Hymn's cost to 2.");

        await CardCmd.Discard(new BlockingPlayerChoiceContext(), hymn);
        await CardPileCmd.Shuffle(new BlockingPlayerChoiceContext(), ctx.Player);
        AutoSlayLog.Info($"[AwakenedTests] backInDraw={ctx.Player.DrawPile.Contains(hymn)} costAfterShuffle={hymn.EnergyCost.GetAmountToSpend()}");
        Assert.IsTrue(ctx.Player.DrawPile.Contains(hymn), "Sanity check: Hymn should be back in the draw pile after reshuffle.");
        Assert.AreEqual(2, hymn.EnergyCost.GetAmountToSpend(), "Sanity check: Hymn's randomized cost should survive the reshuffle.");

        var clutch = await ctx.AddCardToHand<Clutch>();
        await ctx.PlayCard(clutch, ctx.Combat.HittableEnemies.First());

        AutoSlayLog.Info($"[AwakenedTests] hymnInHand={ctx.Player.Hand.Contains(hymn)}");
        Assert.IsTrue(!ctx.Player.Hand.Contains(hymn),
            "Clutch should not pick a card that Snecko Eye randomized to a non-zero cost.");
    }
}
