using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using SlimeBoss.SlimeBossCode.Cards.Token;
using SlimeBoss.SlimeBossCode.Cards.Uncommon;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Slimes;

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

    // Regression guard for a report that Equalize gets stuck when Consume triggers its replay if the
    // FIRST hit already kills the enemy (base 8 + 4 Goop = 12 lethal damage against a 10 HP target).
    // Killing the only enemy ends combat. Vanilla's own multi-play loop (CardModel.OnPlayWrapper)
    // checks IsOverOrEnding before each extra iteration and stops entirely; Equalize's manual "replay
    // via repeating attack+heal in place" didn't, so it fired a pointless second attack (no valid
    // target - AttackCommand.Execute is a no-op) into the now-ending combat, which - combined with
    // CardCmd.Exhaust also no-opping once combat IsOverOrEnding - left the card stuck in the
    // transient Play pile. The fix skips only the now-pointless second attack; the second Heal still
    // applies, since CreatureCmd.Heal explicitly still heals players even once combat IsEnding.
    [CardTest(typeof(SlimeBoss.SlimeBossCode.Core.SlimeBoss))]
    public async Task EqualizeStillHealsTwiceWhenFirstHitEndsCombat(TestContext ctx)
    {
        var enemy = ctx.Combat.HittableEnemies.First();
        enemy.MaxHp = 10;
        enemy.CurrentHp = 10;

        var owner = ctx.Player.Creature;
        owner.CurrentHp = Math.Max(1, owner.MaxHp - 20); // leave room to observe the doubled Heal

        var lick = await ctx.AddCardToHand<Lick>();
        await ctx.PlayCard(lick, enemy);

        var equalize = await ctx.AddCardToHand<Equalize>();
        var startingOwnerHp = owner.CurrentHp;

        await ctx.PlayCard(equalize, enemy);

        Assert.IsTrue(!enemy.IsAlive, "The lethal first hit (8 + 4 Goop) should have killed the enemy.");
        // The exact delta includes some incidental healing from SlimeBoss's default kit unrelated to
        // this card, so this doesn't assert the raw base Heal value (4) directly - only that
        // CreatureCmd.Heal still ran a second time despite the enemy/combat already being gone,
        // which roughly doubles whatever a single call's delta would be.
        Assert.IsTrue(owner.CurrentHp - startingOwnerHp >= 8,
            $"Heal should still apply twice even though the enemy died on the first hit, got a delta of only {owner.CurrentHp - startingOwnerHp}.");
    }

    // Regression guard: DynamicVar.ToString() returns the raw, hook-unmodified BaseValue (IntValue),
    // not PreviewValue - so a bare "{CalculatedBlock}" in a loc string (as ServeProtect's used to be)
    // always shows the un-Frailed amount, even though CalculatedBlockVar.UpdateCardPreview correctly
    // computes the Frail-reduced PreviewValue via Hook.ModifyBlock. The fix uses the ":preview()"
    // SmartFormat formatter (which does read PreviewValue), matching the pattern already used
    // elsewhere (e.g. COLLECTOR-WILDFIRE's "{Hits:preview()}").
    [CardTest(typeof(SlimeBoss.SlimeBossCode.Core.SlimeBoss))]
    public async Task ServeProtectDescriptionShowsFrailReducedBlock(TestContext ctx)
    {
        await SlimeQueue.AddSlime<BronzeSlime>(ctx.Player);
        await SlimeQueue.AddSlime<BronzeSlime>(ctx.Player);
        await PowerCmd.Apply<FrailPower>(new BlockingPlayerChoiceContext(), ctx.Player.Creature, 1,
            ctx.Player.Creature, null);

        var card = await ctx.AddCardToHand<ServeProtect>();
        card.UpdateDynamicVarPreview(CardPreviewMode.Normal, null, card.DynamicVars);
        var text = card.GetDescriptionForPile(PileType.Hand);

        // 2 Slimes * 10 Block = 20 base, reduced 25% by Frail = 15.
        Assert.IsTrue(text.Contains("15"), $"Description should show the Frail-reduced Block (15), got: {text}");
        Assert.IsTrue(!text.Contains("20"), $"Description should not show the un-Frailed Block (20), got: {text}");
    }
}
