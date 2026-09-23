using Awakened.AwakenedCode.Cards.Basic;
using Awakened.AwakenedCode.Cards.Common;
using MegaCrit.Sts2.Core.AutoSlay;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
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

    // Regression guard: reported by a player - Vigor's damage bonus was only applying to Recitation's
    // base attack, not to the extra attack triggered by Chant, even though both hits are shown on the
    // card and should each get the bonus. Root cause was that the chant used a second, separate
    // AttackCommand - Vigor is consumed after one AttackCommand.Execute(), so only the first hit got
    // it. Fixed by dealing both hits from a single AttackCommand (hitCount: 2) when chanting.
    // HasChanted is set directly (rather than playing a Power first) so this isolates Vigor's
    // per-hit behavior from unrelated turn state.
    [CardTest(typeof(Awakened.AwakenedCode.Core.Awakened))]
    public async Task VigorAppliesToBothRecitationHitsWhenChanted(TestContext ctx)
    {
        var enemy = ctx.Combat.HittableEnemies.First();
        var startHp = enemy.CurrentHp;

        await PowerCmd.Apply<VigorPower>(new BlockingPlayerChoiceContext(), ctx.Player.Creature, 5,
            ctx.Player.Creature, null);

        var recitation = (Recitation)await ctx.AddCardToHand<Recitation>();
        recitation.HasChanted = true;
        await ctx.PlayCard(recitation, enemy);

        var totalDamage = startHp - enemy.CurrentHp;
        Assert.AreEqual(22, totalDamage,
            "Vigor should boost both the base attack and the chant-triggered attack from Recitation " +
            "((6 base + 5 vigor) * 2 hits = 22).");
    }
}
