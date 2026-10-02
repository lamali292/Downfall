using BaseLib.Abstracts;
using Downfall.DownfallCode.Compatibility;
using Downfall.DownfallCode.Powers;
using Hermit.HermitCode.Cards.Common;
using Hermit.HermitCode.Cards.Multiplayer;
using Hermit.HermitCode.Cards.Rare;
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
    private static int DeadOnEntries(CardModel card) =>
        CombatManager.Instance.History.Entries.OfType<DeadOnEntry>().Count(e => e.CardPlay.Card == card);

    [CardTest(typeof(Hermit.HermitCode.Core.Hermit))]
    public async Task DeadOnCardPlayedFromCenterTriggers(TestContext ctx)
    {
        await ctx.ClearHand();
        var dive = await ctx.AddCardToHand<Dive>();
        Assert.IsTrue(HermitCmd.IsDeadOn(dive), "Dive alone in hand should be Dead On.");
        await ctx.PlayCard(dive);
        AutoSlayLog.Info($"[HermitTests] (plain) deadOnEntries(dive)={DeadOnEntries(dive)} plated={ctx.Player.Creature.GetPowerAmount<PlatedArmorPower>()}");
        Assert.AreEqual(1, DeadOnEntries(dive), "Dive's Dead On should have triggered.");
        Assert.AreEqual(1, ctx.Player.Creature.GetPowerAmount<PlatedArmorPower>(), "Plated Armor expected.");
    }

    [CardTest(typeof(Hermit.HermitCode.Core.Hermit))]
    public async Task CheatDeadOnTriggersSelectedCardsDeadOn(TestContext ctx)
    {
        await ctx.ClearHand();
        var dive = await ctx.AddCardToTopOfDraw<Dive>();
        var cheat = await ctx.AddCardToHand<Cheat>();

        AutoSlayLog.Info($"[HermitTests] Cheat in hand dead on: {HermitCmd.IsDeadOn(cheat)}");
        Assert.IsTrue(HermitCmd.IsDeadOn(cheat), "Cheat alone in hand should be Dead On.");

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
        await ctx.ClearHand();
        var dive = await ctx.AddCardToTopOfDraw<Dive>();
        // 3 cards, Cheat last → index 2, center is index 1 → not Dead On.
        await ctx.AddCardToHand<Dive>();
        await ctx.AddCardToHand<Dive>();
        var cheat = await ctx.AddCardToHand<Cheat>();
        Assert.IsTrue(!HermitCmd.IsDeadOn(cheat), "Cheat at the edge of hand should not be Dead On.");

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
        await ctx.ClearHand();
        var bullet = await ctx.AddCardToHand<RubberBullet>();
        var baseDamage = bullet.DynamicVars.Damage.BaseValue;
        var increase = bullet.DynamicVars["Increase"].BaseValue;

        await ctx.PlayCard(bullet, ctx.Combat.HittableEnemies.First());

        var moved = RubberBulletInHandOf(teammate);
        var stayed = RubberBulletInHandOf(ctx.Player);
        AutoSlayLog.Info($"[HermitTests] rubber bullet: moved={moved != null} dmg={moved?.DynamicVars.Damage.BaseValue} " +
                         $"ownerHand={stayed != null} deadOnEntries={DeadOnEntries(bullet)}");

        if (CardPlayLocationCompat.SupportsCrossPlayerRedirect)
        {
            // New engine: the Dead On redirect carries a Player, so the card actually moves.
            Assert.IsTrue(moved != null, "Rubber Bullet should be in the teammate's hand.");
            Assert.IsTrue(stayed == null, "Owner should no longer hold a Rubber Bullet.");
            Assert.AreEqual(baseDamage + increase, moved!.DynamicVars.Damage.BaseValue, "Damage should be increased once.");
            Assert.AreEqual(1, teammate.Hand.Count(c => c is RubberBullet), "Exactly one copy should exist.");
        }
        else
        {
            // Old engine: Hook.ModifyCardPlayResultPileTypeAndPosition has no Player, so
            // ModifyCardPlayResultLocationOldPatch drops the redirect and the card stays put
            // (see ModifyCardPlayResultLocationPatch.cs) - only the damage increase still applies.
            Assert.IsTrue(moved == null, "Old engine can't redirect to the teammate's hand.");
            Assert.IsTrue(stayed != null, "Rubber Bullet should stay in the owner's hand on the old engine.");
            Assert.AreEqual(baseDamage + increase, stayed!.DynamicVars.Damage.BaseValue, "Damage should be increased once.");
        }
    }

    [CardTest(typeof(Hermit.HermitCode.Core.Hermit), playerCount: 2)]
    public async Task RubberBulletDeadOnWithSnipeIncreasesDamageTwice(TestContext ctx)
    {
        var teammate = ctx.Players[1];
        await ctx.ClearHand();
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
        if (CardPlayLocationCompat.SupportsCrossPlayerRedirect)
            Assert.IsTrue(copies[0].Owner == teammate, "The single copy should be in the teammate's hand.");
        else
            // Old engine: Hook.ModifyCardPlayResultPileTypeAndPosition has no Player, so the
            // redirect to the teammate is dropped (see ModifyCardPlayResultLocationPatch.cs) -
            // the card stays with its original owner, only the damage stacks.
            Assert.IsTrue(copies[0].Owner == ctx.Player, "Old engine can't redirect to the teammate's hand.");
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
        await ctx.ClearHand();
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
        await ctx.ClearHand();
        var dive = await ctx.AddCardToTopOfDraw<Dive>();
        var cheat = await ctx.AddCardToHand<Cheat>();
        var teammateStrike = await ctx.AddCardToHand<Hermit.HermitCode.Cards.Basic.StrikeHermit>(teammate);

        var cheatPlay = ctx.PlayCard(cheat);
        await ctx.PlayCard(teammateStrike, ctx.Combat.HittableEnemies.First());
        await cheatPlay;

        Assert.AreEqual(1, DeadOnEntries(dive), "Dive's Dead On should trigger once via Cheat.");
        Assert.AreEqual(1, DeadOnEntries(cheat), "Cheat's own Dead On should be recorded.");
    }

    // ---- Spyglass + Replay ----
    // Discord report (Collector Beta): Spyglass's Dead On count advances once per
    // *replay instance*, not once per physical card played. That desyncs from
    // DeadOnPatch's snapshot, which is taken once before the whole (possibly replayed)
    // play - so the "is this card Dead On" answer got frozen at the wrong moment.

    // Dive is added first and never played until last, and enough filler cards stay in
    // hand alongside it, so it always sits away from hand-center - only Spyglass's
    // dynamic per-turn count (not hand-position Dead On) is exercised by these tests.

    [CardTest(typeof(Hermit.HermitCode.Core.Hermit))]
    public async Task SpyglassReplayTriggersDeadOnOnceWhenThresholdReachedMidReplay(TestContext ctx)
    {
        await RelicCmd.Obtain<Spyglass>(ctx.Player);
        await ctx.ClearHand();
        var enemy = ctx.Combat.HittableEnemies.First();

        var dive = await ctx.AddCardToHand<Dive>();
        dive.BaseReplayCount = 1;
        var filler1 = await ctx.AddCardToHand<Hermit.HermitCode.Cards.Basic.StrikeHermit>();
        await ctx.AddCardToHand<Hermit.HermitCode.Cards.Basic.StrikeHermit>();
        await ctx.AddCardToHand<Hermit.HermitCode.Cards.Basic.StrikeHermit>();
        Assert.IsTrue(!HermitCmd.IsDeadOn(dive), "Dive should not be hand-center Dead On here.");

        // 1st card played this turn.
        await ctx.PlayCard(filler1, enemy);

        // 2nd physical card, replayed once (playCount 2): Spyglass's 3rd-play
        // threshold is only reached mid-way through this card's own replay.
        await ctx.PlayCard(dive);

        AutoSlayLog.Info($"[HermitTests] spyglass replay (2nd card, +1 replay): deadOnEntries={DeadOnEntries(dive)}");
        Assert.AreEqual(1, DeadOnEntries(dive),
            "Dead On should trigger exactly once, on the replay instance that reaches the 3rd play.");
    }

    [CardTest(typeof(Hermit.HermitCode.Core.Hermit))]
    public async Task SpyglassReplayTriggersDeadOnOnlyOnceWhenCardIsThirdPlay(TestContext ctx)
    {
        await RelicCmd.Obtain<Spyglass>(ctx.Player);
        await ctx.ClearHand();
        var enemy = ctx.Combat.HittableEnemies.First();

        var dive = await ctx.AddCardToHand<Dive>();
        dive.BaseReplayCount = 1;
        var filler1 = await ctx.AddCardToHand<Hermit.HermitCode.Cards.Basic.StrikeHermit>();
        var filler2 = await ctx.AddCardToHand<Hermit.HermitCode.Cards.Basic.StrikeHermit>();
        await ctx.AddCardToHand<Hermit.HermitCode.Cards.Basic.StrikeHermit>();
        await ctx.AddCardToHand<Hermit.HermitCode.Cards.Basic.StrikeHermit>();
        Assert.IsTrue(!HermitCmd.IsDeadOn(dive), "Dive should not be hand-center Dead On here.");

        await ctx.PlayCard(filler1, enemy);
        await ctx.PlayCard(filler2, enemy);

        // 3rd physical card, replayed once: only the FIRST instance should be Dead On,
        // not both copies of the replay.
        await ctx.PlayCard(dive);

        AutoSlayLog.Info($"[HermitTests] spyglass replay (3rd card, +1 replay): deadOnEntries={DeadOnEntries(dive)}");
        Assert.AreEqual(1, DeadOnEntries(dive),
            "Dead On should trigger exactly once even though the 3rd card is replayed.");
    }

    // ---- Dead On single query: in hand, Play pile (after-play handler), replay ----

    [CardTest(typeof(Hermit.HermitCode.Core.Hermit))]
    public async Task DeadOnQueryInHandCenterVersusEdge(TestContext ctx)
    {
        await ctx.ClearHand();
        var a = await ctx.AddCardToHand<Dive>();
        var b = await ctx.AddCardToHand<Dive>();
        var c = await ctx.AddCardToHand<Dive>();
        Assert.IsTrue(!HermitCmd.IsDeadOn(a), "Left edge is not Dead On.");
        Assert.IsTrue(HermitCmd.IsDeadOn(b), "Center of an odd hand is Dead On.");
        Assert.IsTrue(!HermitCmd.IsDeadOn(c), "Right edge is not Dead On.");
        var d = await ctx.AddCardToHand<Dive>();
        var hand = ctx.Player.Hand.ToList();
        Assert.AreEqual(2, hand.Count(HermitCmd.IsDeadOn), "Even hand has two Dead On cards.");
        Assert.IsTrue(HermitCmd.HasActiveDeadOnEffect(hand[1]) && HermitCmd.HasActiveDeadOnEffect(hand[2]),
            "Middle two Dives have an active Dead On effect.");
        Assert.IsTrue(!HermitCmd.HasActiveDeadOnEffect(hand[0]), "Edge Dive has no active Dead On effect.");
    }

    // The card is in the Play pile when the after-play handler asks: the answer must come from the
    // pre-play hand snapshot, and must not carry over to a later play of the same card instance.
    [CardTest(typeof(Hermit.HermitCode.Core.Hermit))]
    public async Task DeadOnSnapshotDoesNotLeakIntoNextPlay(TestContext ctx)
    {
        await ctx.ClearHand();
        var dive = await ctx.AddCardToHand<Dive>();
        await ctx.PlayCard(dive);
        Assert.AreEqual(1, DeadOnEntries(dive), "Centered Dive triggers from the Play pile.");

        await ctx.ClearHand();
        await ctx.AddCardToHand<Dive>();
        await ctx.AddCardToHand<Dive>();
        await ctx.AddCardToHand<Dive>();
        await CardPileCmd.Add(dive, PileType.Hand);
        Assert.IsTrue(!HermitCmd.IsDeadOn(dive), "Dive re-added at the edge is not Dead On.");
        await ctx.PlayCard(dive);
        Assert.AreEqual(1, DeadOnEntries(dive), "Edge replay of the same instance must not trigger Dead On.");
    }

    // A hand-center card that is replayed: both replay instances are Dead On (snapshot is per card).
    [CardTest(typeof(Hermit.HermitCode.Core.Hermit))]
    public async Task DeadOnCenterCardTriggersOnEveryReplayInstance(TestContext ctx)
    {
        await ctx.ClearHand();
        var dive = await ctx.AddCardToHand<Dive>();
        dive.BaseReplayCount = 1;
        await ctx.PlayCard(dive);
        Assert.AreEqual(2, DeadOnEntries(dive), "Both instances of a centered, replayed Dive are Dead On.");
    }

    // Vantage's Dead On effect draws, so the hand changes while the play is still running. The card
    // leaves the hand at play start; its status must come from that moment, for every replay instance.
    [CardTest(typeof(Hermit.HermitCode.Core.Hermit))]
    public async Task DeadOnReplayedPlayIsNotAlteredByHandChangeMidPlay(TestContext ctx)
    {
        await ctx.ClearHand();
        await ctx.AddCardToHand<Hermit.HermitCode.Cards.Basic.StrikeHermit>();
        var vantage = await ctx.AddCardToHand<Vantage>();
        await ctx.AddCardToHand<Hermit.HermitCode.Cards.Basic.StrikeHermit>();
        vantage.BaseReplayCount = 1;
        Assert.IsTrue(HermitCmd.IsDeadOn(vantage), "Vantage in the middle of three cards is Dead On.");

        await ctx.PlayCard(vantage);

        Assert.IsTrue(ctx.Player.Hand.Count() > 2, "Vantage's Dead On effect should have drawn cards mid-play.");
        Assert.AreEqual(2, DeadOnEntries(vantage),
            "Both replay instances are Dead On even though the first one changed the hand.");
    }

    // CursedSkull's DeadOnReplay modifier attaches to a card with no Dead On effect of its own
    // (Strike) - it only ever contributes extra plays, and records its own DeadOnEntry for Called
    // Shot/Combo-style readers, while the modified card happens to be Dead On.
    [CardTest(typeof(Hermit.HermitCode.Core.Hermit))]
    public async Task DeadOnReplayModifierAddsAPlayOnlyWhileDeadOn(TestContext ctx)
    {
        await ctx.ClearHand();
        var strike = await ctx.AddCardToHand<Hermit.HermitCode.Cards.Basic.StrikeHermit>();
        CardModifier.AddModifier<DeadOnReplay>(strike);

        await ctx.ClearHand();
        await ctx.AddCardToHand<Hermit.HermitCode.Cards.Basic.DefendHermit>();
        await CardPileCmd.Add(strike, PileType.Hand);
        await ctx.AddCardToHand<Hermit.HermitCode.Cards.Basic.DefendHermit>();
        Assert.IsTrue(HermitCmd.IsDeadOn(strike), "Strike in the middle of three cards is Dead On.");
        await ctx.PlayCard(strike);
        Assert.AreEqual(2, DeadOnEntries(strike), "DeadOnReplay granted one extra play while Dead On.");

        await ctx.ClearHand();
        await CardPileCmd.Add(strike, PileType.Hand);
        await ctx.AddCardToHand<Hermit.HermitCode.Cards.Basic.DefendHermit>();
        await ctx.AddCardToHand<Hermit.HermitCode.Cards.Basic.DefendHermit>();
        Assert.IsTrue(!HermitCmd.IsDeadOn(strike), "Strike at the edge is not Dead On.");
        await ctx.PlayCard(strike);
        Assert.AreEqual(2, DeadOnEntries(strike),
            "No extra play granted while not Dead On - still just the 2 entries from the first play.");
    }

    // Called Shot's own "did my last play trigger Dead On" check reads DeadOnEntry history, not
    // keywords - DeadOnReplay-granted plays must record their own entry for it to see.
    [CardTest(typeof(Hermit.HermitCode.Core.Hermit))]
    public async Task DeadOnReplayModifierRecordsEntryForCalledShotToRead(TestContext ctx)
    {
        await ctx.ClearHand();
        await ctx.AddCardToHand<Hermit.HermitCode.Cards.Basic.DefendHermit>();
        var strike = await ctx.AddCardToHand<Hermit.HermitCode.Cards.Basic.StrikeHermit>();
        CardModifier.AddModifier<DeadOnReplay>(strike);
        await ctx.AddCardToHand<Hermit.HermitCode.Cards.Basic.DefendHermit>();
        Assert.IsTrue(HermitCmd.IsDeadOn(strike), "Strike in the middle of three cards is Dead On.");
        await ctx.PlayCard(strike);

        var calledShot = await ctx.AddCardToHand<CalledShot>();
        var handCountBeforeCalledShot = ctx.Player.Hand.Count();
        await ctx.PlayCard(calledShot, ctx.Combat.HittableEnemies.First());
        Assert.AreEqual(handCountBeforeCalledShot, ctx.Player.Hand.Count(),
            "Called Shot should draw a card: the immediately preceding play (DeadOnReplay's Strike) triggered Dead On.");
    }

    // ---- Curse adjacency ----

    [CardTest(typeof(Hermit.HermitCode.Core.Hermit))]
    public async Task CurseAdjacencyQueryInHand(TestContext ctx)
    {
        await ctx.ClearHand();
        var left = await ctx.AddCardToHand<Dive>();
        await ctx.AddCardToHand<Hermit.HermitCode.Cards.Curse.ImpendingDoom>();
        var right = await ctx.AddCardToHand<Dive>();
        var far = await ctx.AddCardToHand<Dive>();
        Assert.IsTrue(HermitCmd.IsAdjacentToCurse(left), "Left neighbour of a curse is adjacent.");
        Assert.IsTrue(HermitCmd.IsAdjacentToCurse(right), "Right neighbour of a curse is adjacent.");
        Assert.IsTrue(!HermitCmd.IsAdjacentToCurse(far), "Two away from the curse is not adjacent.");
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
