using Downfall.DownfallCode.Powers;
using Hermit.HermitCode.Cards.Common;
using Hermit.HermitCode.Cards.Multiplayer;
using Hermit.HermitCode.Cards.Uncommon;
using Hermit.HermitCode.Core;
using Hermit.HermitCode.History;
using Hermit.HermitCode.Powers;
using Hermit.HermitCode.Relics;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.AutoSlay;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.TestCode;

public class HermitTests
{
    /// Empties the hand so the next card added is guaranteed to be the center card.
    private static async Task ClearHand(TestContext ctx)
    {
        var hand = PileType.Hand.GetPile(ctx.Player).Cards.ToList();
        if (hand.Count > 0) await CardPileCmd.Add(hand, PileType.Discard);
    }

    private static int DeadOnEntries(CardModel card) =>
        CombatManager.Instance.History.Entries.OfType<DeadOnEntry>().Count(e => e.CardPlay.Card == card);

    [CardTest(typeof(Hermit.HermitCode.Core.Hermit))]
    public async Task DeadOnCardPlayedFromCenterTriggers(TestContext ctx)
    {
        await ClearHand(ctx);
        var dive = await ctx.AddCardToHand<Dive>();
        Assert.IsTrue(HermitCmd.IsDeadOnInCurrentHandState(dive), "Dive alone in hand should be Dead On.");
        await ctx.PlayCard(dive);
        AutoSlayLog.Info($"[HermitTests] (plain) deadOnEntries(dive)={DeadOnEntries(dive)} plated={ctx.Player.Creature.GetPowerAmount<PlatedArmorPower>()}");
        Assert.AreEqual(1, DeadOnEntries(dive), "Dive's Dead On should have triggered.");
        Assert.AreEqual(1, ctx.Player.Creature.GetPowerAmount<PlatedArmorPower>(), "Plated Armor expected.");
    }

    [CardTest(typeof(Hermit.HermitCode.Core.Hermit))]
    public async Task CheatDeadOnTriggersSelectedCardsDeadOn(TestContext ctx)
    {
        await ClearHand(ctx);
        var dive = await ctx.AddCardToTopOfDraw<Dive>();
        var cheat = await ctx.AddCardToHand<Cheat>();

        AutoSlayLog.Info($"[HermitTests] Cheat in hand dead on: {HermitCmd.IsDeadOnInCurrentHandState(cheat)}");
        Assert.IsTrue(HermitCmd.IsDeadOnInCurrentHandState(cheat), "Cheat alone in hand should be Dead On.");

        await ctx.PlayCard(cheat);

        AutoSlayLog.Info($"[HermitTests] dive pile={dive.Pile?.Type} cheatPowerLeft={ctx.Player.Creature.HasPower<CheatPower>()} " +
                         $"deadOnEntries(dive)={DeadOnEntries(dive)} deadOnEntries(cheat)={DeadOnEntries(cheat)} " +
                         $"plated={ctx.Player.Creature.GetPowerAmount<PlatedArmorPower>()}");

        Assert.IsTrue(dive.Pile?.Type == PileType.Discard, "Dive should have been auto-played by Cheat.");
        Assert.AreEqual(1, DeadOnEntries(dive), "Dive's Dead On should have triggered exactly once.");
        Assert.AreEqual(1, ctx.Player.Creature.GetPowerAmount<PlatedArmorPower>(),
            "Dive's Dead On effect (Plated Armor) should have applied.");
        Assert.IsTrue(!ctx.Player.Creature.HasPower<CheatPower>(), "Cheat power should be consumed.");
    }

    [CardTest(typeof(Hermit.HermitCode.Core.Hermit))]
    public async Task CheatNotDeadOnDoesNotTriggerSelectedCardsDeadOn(TestContext ctx)
    {
        await ClearHand(ctx);
        var dive = await ctx.AddCardToTopOfDraw<Dive>();
        // 3 cards, Cheat last → index 2, center is index 1 → not Dead On.
        await ctx.AddCardToHand<Dive>();
        await ctx.AddCardToHand<Dive>();
        var cheat = await ctx.AddCardToHand<Cheat>();
        Assert.IsTrue(!HermitCmd.IsDeadOnInCurrentHandState(cheat), "Cheat at the edge of hand should not be Dead On.");

        await ctx.PlayCard(cheat);

        AutoSlayLog.Info($"[HermitTests] (not dead on) dive pile={dive.Pile?.Type} deadOnEntries(dive)={DeadOnEntries(dive)} " +
                         $"plated={ctx.Player.Creature.GetPowerAmount<PlatedArmorPower>()}");

        Assert.IsTrue(dive.Pile?.Type == PileType.Discard, "Dive should have been auto-played by Cheat.");
        Assert.AreEqual(0, DeadOnEntries(dive), "Dive's Dead On should not trigger when Cheat wasn't Dead On.");
        Assert.AreEqual(0, ctx.Player.Creature.GetPowerAmount<PlatedArmorPower>(), "No Plated Armor expected.");
    }

    // ---- multiplayer ----

    private static CardModel? RubberBulletInHandOf(Player player) =>
        player.Hand.FirstOrDefault(c => c is RubberBullet);

    [CardTest(typeof(Hermit.HermitCode.Core.Hermit), playerCount: 2)]
    public async Task RubberBulletDeadOnMovesToTeammateWithIncreasedDamage(TestContext ctx)
    {
        var teammate = ctx.Players[1];
        await ClearHand(ctx);
        var bullet = await ctx.AddCardToHand<RubberBullet>();
        var baseDamage = bullet.DynamicVars.Damage.BaseValue;
        var increase = bullet.DynamicVars["Increase"].BaseValue;

        await ctx.PlayCard(bullet, ctx.Combat.HittableEnemies.First());

        var moved = RubberBulletInHandOf(teammate);
        AutoSlayLog.Info($"[HermitTests] rubber bullet: moved={moved != null} dmg={moved?.DynamicVars.Damage.BaseValue} " +
                         $"ownerHand={RubberBulletInHandOf(ctx.Player) != null} deadOnEntries={DeadOnEntries(bullet)}");
        Assert.IsTrue(moved != null, "Rubber Bullet should be in the teammate's hand.");
        Assert.IsTrue(RubberBulletInHandOf(ctx.Player) == null, "Owner should no longer hold a Rubber Bullet.");
        Assert.AreEqual(baseDamage + increase, moved!.DynamicVars.Damage.BaseValue, "Damage should be increased once.");
        Assert.AreEqual(1, teammate.Hand.Count(c => c is RubberBullet), "Exactly one copy should exist.");
    }

    [CardTest(typeof(Hermit.HermitCode.Core.Hermit), playerCount: 2)]
    public async Task RubberBulletDeadOnWithSnipeIncreasesDamageTwice(TestContext ctx)
    {
        var teammate = ctx.Players[1];
        await ClearHand(ctx);
        await PowerCmd.Apply<SnipePower>(new BlockingPlayerChoiceContext(), ctx.Player.Creature, 1, ctx.Player.Creature, null);
        var bullet = await ctx.AddCardToHand<RubberBullet>();
        var baseDamage = bullet.DynamicVars.Damage.BaseValue;
        var increase = bullet.DynamicVars["Increase"].BaseValue;

        await ctx.PlayCard(bullet, ctx.Combat.HittableEnemies.First());

        var copies = ctx.Players.SelectMany(p => p.Hand).Where(c => c is RubberBullet).ToList();
        AutoSlayLog.Info($"[HermitTests] rubber bullet + snipe: copies={copies.Count} " +
                         $"dmg=[{string.Join(",", copies.Select(c => c.DynamicVars.Damage.BaseValue))}] " +
                         $"snipeLeft={ctx.Player.Creature.HasPower<SnipePower>()}");
        Assert.AreEqual(1, copies.Count, "Exactly one Rubber Bullet should exist after a double Dead On.");
        Assert.IsTrue(copies[0].Owner == teammate, "The single copy should be in the teammate's hand.");
        Assert.AreEqual(baseDamage + 2 * increase, copies[0].DynamicVars.Damage.BaseValue,
            "Snipe should have applied the damage increase twice.");
        Assert.IsTrue(!ctx.Player.Creature.HasPower<SnipePower>(), "Snipe should be consumed.");
    }

    // Combo redirects a Dead On card back into the owner's own hand - Rubber Bullet's own Dead On
    // hand-off must not fight that over the same card (Combo wins, matching other Dead On
    // multiplayer cards).
    [CardTest(typeof(Hermit.HermitCode.Core.Hermit), playerCount: 2)]
    public async Task RubberBulletDeadOnStaysInOwnHandWithCombo(TestContext ctx)
    {
        var teammate = ctx.Players[1];
        await ClearHand(ctx);
        await PowerCmd.Apply<ComboPower>(new BlockingPlayerChoiceContext(), ctx.Player.Creature, 1, ctx.Player.Creature, null);
        var bullet = await ctx.AddCardToHand<RubberBullet>();
        var baseDamage = bullet.DynamicVars.Damage.BaseValue;
        var increase = bullet.DynamicVars["Increase"].BaseValue;

        await ctx.PlayCard(bullet, ctx.Combat.HittableEnemies.First());

        var copies = ctx.Players.SelectMany(p => p.Hand).Where(c => c is RubberBullet).ToList();
        AutoSlayLog.Info($"[HermitTests] rubber bullet + combo: copies={copies.Count} " +
                         $"ownerHand={RubberBulletInHandOf(ctx.Player) != null} teammateHand={RubberBulletInHandOf(teammate) != null}");
        Assert.AreEqual(1, copies.Count, "Exactly one Rubber Bullet should exist.");
        Assert.IsTrue(copies[0].Owner == ctx.Player, "Combo should keep Rubber Bullet in the owner's own hand.");
        Assert.AreEqual(baseDamage + increase, copies[0].DynamicVars.Damage.BaseValue,
            "Damage should still be increased once even though the hand-off was skipped.");
    }

    [CardTest(typeof(Hermit.HermitCode.Core.Hermit), playerCount: 2)]
    public async Task CheatDeadOnWorksWhileTeammatePlaysCards(TestContext ctx)
    {
        // A teammate's card play between Cheat's snapshot and its after-play handler must not
        // disturb Cheat's Dead On state (the old process-wide statics broke here in multiplayer).
        var teammate = ctx.Players[1];
        await ClearHand(ctx);
        var dive = await ctx.AddCardToTopOfDraw<Dive>();
        var cheat = await ctx.AddCardToHand<Cheat>();
        var teammateStrike = await ctx.AddCardToHand<Hermit.HermitCode.Cards.Basic.StrikeHermit>(teammate);

        var cheatPlay = ctx.PlayCard(cheat);
        await ctx.PlayCard(teammateStrike, ctx.Combat.HittableEnemies.First());
        await cheatPlay;

        Assert.AreEqual(1, DeadOnEntries(dive), "Dive's Dead On should trigger once via Cheat.");
        Assert.AreEqual(1, DeadOnEntries(cheat), "Cheat's own Dead On should be recorded.");
    }

    // ---- Red Scarf ----

    [CardTest(typeof(Hermit.HermitCode.Core.Hermit))]
    public async Task RedScarfGainsBlockOnNewEnemyDebuff(TestContext ctx)
    {
        await RelicCmd.Obtain<RedScarf>(ctx.Player);
        var enemy = ctx.Combat.HittableEnemies.First();
        var startBlock = ctx.Player.Creature.Block;

        await PowerCmd.Apply<WeakPower>(new BlockingPlayerChoiceContext(), enemy, 1, ctx.Player.Creature, null);

        AutoSlayLog.Info($"[HermitTests] RedScarf (no Artifact): block {startBlock} -> {ctx.Player.Creature.Block} " +
                         $"weak={enemy.GetPowerAmount<WeakPower>()}");
        Assert.AreEqual(1, enemy.GetPowerAmount<WeakPower>(), "Weak should have been applied.");
        Assert.AreEqual(startBlock + 3, ctx.Player.Creature.Block, "Red Scarf should grant 3 Block for a new debuff.");
    }

    [CardTest(typeof(Hermit.HermitCode.Core.Hermit))]
    public async Task RedScarfDoesNotGainBlockWhenArtifactBlocksDebuff(TestContext ctx)
    {
        await RelicCmd.Obtain<RedScarf>(ctx.Player);
        var enemy = ctx.Combat.HittableEnemies.First();
        await PowerCmd.Apply<ArtifactPower>(new BlockingPlayerChoiceContext(), enemy, 1, enemy, null);
        var startBlock = ctx.Player.Creature.Block;

        await PowerCmd.Apply<WeakPower>(new BlockingPlayerChoiceContext(), enemy, 1, ctx.Player.Creature, null);

        AutoSlayLog.Info($"[HermitTests] RedScarf (with Artifact): block {startBlock} -> {ctx.Player.Creature.Block} " +
                         $"artifactLeft={enemy.GetPowerAmount<ArtifactPower>()} weak={enemy.GetPowerAmount<WeakPower>()}");
        Assert.AreEqual(0, enemy.GetPowerAmount<WeakPower>(), "Weak should have been fully blocked by Artifact.");
        Assert.AreEqual(startBlock, ctx.Player.Creature.Block,
            "Red Scarf should not grant Block when Artifact blocks the debuff.");
    }
}
