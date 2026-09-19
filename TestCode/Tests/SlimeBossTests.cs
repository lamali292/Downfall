using MegaCrit.Sts2.Core.Entities.Cards;
using SlimeBoss.SlimeBossCode.Cards.Token;
using SlimeBoss.SlimeBossCode.Cards.Uncommon;

namespace Downfall.TestCode;

public class SlimeBossTests
{
    // Regression guard: Equalize's Consume effect used to call CardCmd.AutoPlay(this) directly from
    // ConsumeEffect, which fires mid-attack (GoopPower.AfterAttack -> ConsumeGoop), reentering the
    // card's own still-executing OnPlayWrapper (and, outside of tests, its still-in-flight NCard/pile
    // visuals) and corrupting its play state - crashing on a later play. The "play this twice" effect
    // is now done by repeating the attack+heal in OnPlayInternal instead of replaying the whole card.
    [CardTest(typeof(SlimeBoss.SlimeBossCode.Core.SlimeBoss))]
    public async Task EqualizeConsumeReplaysWithoutCorruptingCardState(TestContext ctx)
    {
        var enemy = ctx.Combat.HittableEnemies.First();
        var lick = await ctx.AddCardToHand<Lick>();
        await ctx.PlayCard(lick, enemy);

        var equalize = await ctx.AddCardToHand<Equalize>();
        var startingHp = enemy.CurrentHp;

        await ctx.PlayCard(equalize, enemy);

        Assert.AreEqual(PileType.Exhaust, equalize.Pile?.Type,
            "Equalize should end up exhausted exactly once, not stuck mid-resolution.");
        Assert.IsTrue(startingHp - enemy.CurrentHp > 12,
            "Equalize should deal damage twice (base 8+4 Goop bonus, then base 8 again) when Consume triggers a replay.");

        // Playing another Equalize afterwards must not crash - guards against corrupted pile/play state
        // from the original bug leaking into later plays.
        var secondEqualize = await ctx.AddCardToHand<Equalize>();
        await ctx.PlayCard(secondEqualize, enemy);

        Assert.AreEqual(PileType.Exhaust, secondEqualize.Pile?.Type,
            "A second Equalize should also resolve and exhaust normally.");
    }
}
