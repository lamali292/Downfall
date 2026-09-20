using BaseLib.Abstracts;
using Guardian.GuardianCode.Cards.Abstract;
using Guardian.GuardianCode.Cards.Basic;
using Guardian.GuardianCode.Cards.Common;
using Guardian.GuardianCode.Cards.Uncommon;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.Enchantments;
using Guardian.GuardianCode.Gems;
using Guardian.GuardianCode.Interfaces;
using Guardian.GuardianCode.Relics;
using MegaCrit.Sts2.Core.AutoSlay;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.TestCode;

public class GuardianTests
{
    /// Empties a player's hand so Curl Up's random-stasis-target selection is deterministic.
    private static async Task ClearHand(TestContext ctx, Player? player = null)
    {
        player ??= ctx.Player;
        var hand = PileType.Hand.GetPile(player).Cards.ToList();
        if (hand.Count > 0) await CardPileCmd.Add(hand, PileType.Discard);
    }

    [CardTest(typeof(Guardian.GuardianCode.Core.Guardian))]
    public async Task RerouteWithCryoChamberUpgradesRoutedCard(TestContext ctx)
    {
        await RelicCmd.Obtain<CryoChamber>(ctx.Player);
        GuardianCmd.AddMaxStasisSlots(ctx.Player);

        await ClearHand(ctx);
        var reroute = await ctx.AddCardToHand<Reroute>();
        var strike = await ctx.AddCardToHand<StrikeGuardian>();
        var enemy = ctx.Combat.HittableEnemies.First();

        await ctx.PlayCard(reroute, enemy);
        await ctx.PlayCard(strike, enemy);

        var stasis = GuardianCmd.GetStasisPile(ctx.Player);
        Assert.IsTrue(stasis.Cards.Contains(strike), "Strike should have been routed into Stasis by Reroute.");
        Assert.IsTrue(strike.IsUpgraded, "CryoChamber should upgrade a card routed into Stasis by Reroute.");
    }

    [CardTest(typeof(Guardian.GuardianCode.Core.Guardian))]
    public async Task RerouteDoesNotOverflowStasisWhenTheRoutedCardAlsoStasisesACard(TestContext ctx)
    {
        // With a single free slot, Curl Up's own effect (stasis Strike) and Reroute's redirect
        // (stasis Curl Up itself) are both trying to claim the same one slot. Pre-fix, both
        // succeeded independently and overflowed the pile to 2/1.
        GuardianCombatModel.StasisSlots[ctx.Player] = 1;

        await ClearHand(ctx);
        var reroute = await ctx.AddCardToHand<Reroute>();
        var curlUp = await ctx.AddCardToHand<CurlUp>();
        var strike = await ctx.AddCardToHand<StrikeGuardian>();
        var enemy = ctx.Combat.HittableEnemies.First();

        await ctx.PlayCard(reroute, enemy);
        await ctx.PlayCard(curlUp);

        var stasis = GuardianCmd.GetStasisPile(ctx.Player);
        var max = GuardianCmd.GetMaxStasisSlots(ctx.Player);
        AutoSlayLog.Info($"[GuardianTests] Stasis after Reroute+CurlUp (1 slot): " +
                         $"[{string.Join(",", stasis.Cards.Select(c => c.GetType().Name))}]");
        Assert.IsTrue(stasis.Cards.Count <= max,
            $"Stasis should never exceed its slot cap (max {max}, got {stasis.Cards.Count}).");
        // Reroute committed to redirecting Curl Up before Curl Up's own effect ran, so by the time
        // Curl Up checks CanPutIntoStasis the slot already reads as claimed: Curl Up wins the race,
        // Strike's stasis attempt is correctly refused, and Strike stays in hand.
        Assert.IsTrue(stasis.Cards.Contains(curlUp), "Curl Up should have been redirected into Stasis by Reroute.");
        Assert.IsTrue(!stasis.Cards.Contains(strike), "Curl Up's own Stasis attempt should have been refused (no room).");
        Assert.IsTrue(ctx.Player.Hand.Contains(strike), "Strike should remain in hand since its Stasis attempt failed.");
    }

    [CardTest(typeof(Guardian.GuardianCode.Core.Guardian))]
    public async Task RerouteAndTheRoutedCardsOwnStasisBothSucceedWhenThereIsRoom(TestContext ctx)
    {
        // Same interaction, but with enough slots for both — the fix must not over-restrict when
        // there's no actual contention.
        GuardianCombatModel.StasisSlots[ctx.Player] = 2;

        await ClearHand(ctx);
        var reroute = await ctx.AddCardToHand<Reroute>();
        var curlUp = await ctx.AddCardToHand<CurlUp>();
        var strike = await ctx.AddCardToHand<StrikeGuardian>();
        var enemy = ctx.Combat.HittableEnemies.First();

        await ctx.PlayCard(reroute, enemy);
        await ctx.PlayCard(curlUp);

        var stasis = GuardianCmd.GetStasisPile(ctx.Player);
        AutoSlayLog.Info($"[GuardianTests] Stasis after Reroute+CurlUp (2 slots): " +
                         $"[{string.Join(",", stasis.Cards.Select(c => c.GetType().Name))}]");
        Assert.AreEqual(2, stasis.Cards.Count, "Both Curl Up and Strike should have entered Stasis.");
        Assert.IsTrue(stasis.Cards.Contains(curlUp), "Curl Up should have been redirected into Stasis by Reroute.");
        Assert.IsTrue(stasis.Cards.Contains(strike), "Curl Up's own effect should have stasis'd Strike.");
    }

    [CardTest(typeof(Guardian.GuardianCode.Core.Guardian))]
    public async Task RerouteDoesNotOverflowStasisAtRealMaxWithSlotsAlreadyFilled(TestContext ctx)
    {
        // Guardian's real starting cap (3), not an artificially tightened one, with 2 of the 3
        // slots already occupied by unrelated cards — only 1 free slot, same contention as the
        // 1-slot test above but reached the way it'd actually happen in a run.
        var max = GuardianCmd.GetMaxStasisSlots(ctx.Player);
        Assert.AreEqual(3, max, "Sanity check: Guardian's starting max Stasis slots is 3.");

        await ClearHand(ctx);
        var filler1 = await ctx.AddCardToHand<DefendGuardian>();
        var filler2 = await ctx.AddCardToHand<DefendGuardian>();
        await GuardianCmd.PutIntoStasis(filler1, new BlockingPlayerChoiceContext(), filler1);
        await GuardianCmd.PutIntoStasis(filler2, new BlockingPlayerChoiceContext(), filler2);

        var reroute = await ctx.AddCardToHand<Reroute>();
        var curlUp = await ctx.AddCardToHand<CurlUp>();
        var strike = await ctx.AddCardToHand<StrikeGuardian>();
        var enemy = ctx.Combat.HittableEnemies.First();

        await ctx.PlayCard(reroute, enemy);
        await ctx.PlayCard(curlUp);

        var stasis = GuardianCmd.GetStasisPile(ctx.Player);
        AutoSlayLog.Info($"[GuardianTests] Stasis at real max (2 pre-filled) after Reroute+CurlUp: " +
                         $"[{string.Join(",", stasis.Cards.Select(c => c.GetType().Name))}] (max {max})");
        Assert.IsTrue(stasis.Cards.Count <= max,
            $"Stasis should never exceed its slot cap (max {max}, got {stasis.Cards.Count}).");
        Assert.IsTrue(stasis.Cards.Contains(curlUp), "Curl Up should have been redirected into the last free slot.");
        Assert.IsTrue(!stasis.Cards.Contains(strike), "Curl Up's own Stasis attempt should have been refused (no room).");
    }

    [CardTest(typeof(Guardian.GuardianCode.Core.Guardian), playerCount: 2)]
    public async Task RerouteStasisReservationDoesNotLeakBetweenPlayers(TestContext ctx)
    {
        // PendingStasisRedirect is a PlayerField (keyed per PlayerCombatState), so this should be
        // trivially isolated — but interleave both players' plays anyway (as
        // HermitTests.CheatDeadOnWorksWhileTeammatePlaysCards does for its own per-player state)
        // to actually exercise that isolation rather than just assert it.
        var teammate = ctx.Players[1];
        GuardianCombatModel.StasisSlots[ctx.Player] = 1;
        GuardianCombatModel.StasisSlots[teammate] = 1;

        await ClearHand(ctx);
        await ClearHand(ctx, teammate);

        var reroute = await ctx.AddCardToHand<Reroute>();
        var curlUp = await ctx.AddCardToHand<CurlUp>();
        var strike = await ctx.AddCardToHand<StrikeGuardian>();
        var teammateReroute = await ctx.AddCardToHand<Reroute>(teammate);
        var teammateCurlUp = await ctx.AddCardToHand<CurlUp>(teammate);
        var teammateStrike = await ctx.AddCardToHand<StrikeGuardian>(teammate);
        var enemy = ctx.Combat.HittableEnemies.First();

        var myReroutePlay = ctx.PlayCard(reroute, enemy);
        var teammateReroutePlay = ctx.PlayCard(teammateReroute, enemy);
        await myReroutePlay;
        await teammateReroutePlay;

        var myCurlUpPlay = ctx.PlayCard(curlUp);
        var teammateCurlUpPlay = ctx.PlayCard(teammateCurlUp);
        await myCurlUpPlay;
        await teammateCurlUpPlay;

        var myStasis = GuardianCmd.GetStasisPile(ctx.Player);
        var teammateStasis = GuardianCmd.GetStasisPile(teammate);
        AutoSlayLog.Info($"[GuardianTests] Interleaved multiplayer Stasis: mine=" +
                         $"[{string.Join(",", myStasis.Cards.Select(c => c.GetType().Name))}] teammate=" +
                         $"[{string.Join(",", teammateStasis.Cards.Select(c => c.GetType().Name))}]");

        Assert.IsTrue(myStasis.Cards.Count <= 1, $"My Stasis should stay within its own cap (got {myStasis.Cards.Count}).");
        Assert.IsTrue(teammateStasis.Cards.Count <= 1,
            $"Teammate's Stasis should stay within its own cap (got {teammateStasis.Cards.Count}).");
        Assert.IsTrue(myStasis.Cards.Contains(curlUp), "My Curl Up should have been redirected into my own Stasis.");
        Assert.IsTrue(teammateStasis.Cards.Contains(teammateCurlUp),
            "Teammate's Curl Up should have been redirected into their own Stasis, unaffected by mine.");
    }

    [CardTest(typeof(Guardian.GuardianCode.Core.Guardian))]
    public async Task TemporalStasisEntryCleansUpAnEarlierVisibleHandPlacement(TestContext ctx)
    {
        // Reported bug: Bottled Black Hole's Temporal enchantment moves its card into Stasis on
        // turn 1 via BeforeHandDrawLate, always skipping pile-change visuals under the assumption
        // the card is still sitting unseen in the Draw pile. Jeweled Mask (a vanilla relic) can
        // put a Power card into Hand — visibly — earlier in that same turn-1 BeforeHandDraw phase.
        // If Temporal then removes it from Hand silently, Hand never fires CardRemoved, leaving a
        // stale on-screen card node behind (the "phantom Orbwalk stuck in hand position #2" the
        // player saw). Simulate that ordering directly against Temporal's own hook.
        await ClearHand(ctx);
        var orbwalk = await ctx.AddCardToHand<Orbwalk>();
        CardCmd.Enchant<Temporal>(orbwalk, 1);

        var removedFromHand = false;
        PileType.Hand.GetPile(ctx.Player).CardRemoved += _ => removedFromHand = true;

        var enchantment = (Temporal)orbwalk.Enchantment!;
        await enchantment.BeforeHandDrawLate(ctx.Player, new BlockingPlayerChoiceContext(), ctx.Combat);

        var stasis = GuardianCmd.GetStasisPile(ctx.Player);
        Assert.IsTrue(stasis.Cards.Contains(orbwalk), "Temporal should have moved the card into Stasis.");
        Assert.IsTrue(removedFromHand,
            "Hand should fire CardRemoved when Temporal pulls a card that was already visibly in Hand, " +
            "or its UI node is orphaned instead of cleaned up.");
    }

    [CardTest(typeof(Guardian.GuardianCode.Core.Guardian))]
    public async Task DowngradingAStackedRefractedBeamOnlyRemovesOneLevel(TestContext ctx)
    {
        // Reported bug: Refracted Beam's MaxUpgradeLevel grows with its own upgrade level (it can
        // sit at +2, +3, ...), but CardModel.DowngradeInternal (used by e.g. the Reflections
        // event) unconditionally resets CurrentUpgradeLevel to 0 - a +3 card was fully wiped to
        // +0 by a single downgrade that should only have removed one level.
        var card = (RefractedBeam)await ctx.AddCardToHand<RefractedBeam>();
        CardCmd.Upgrade(card);
        CardCmd.Upgrade(card);
        Assert.AreEqual(2, card.CurrentUpgradeLevel, "Sanity check: card should be +2 before downgrading.");

        CardCmd.Downgrade(card);

        Assert.AreEqual(1, card.CurrentUpgradeLevel,
            "Downgrading a stacked Refracted Beam should remove exactly one level, not reset it to +0.");

        // The fix works by fully resetting the card (the game's own DowngradeInternal) and then
        // replaying upgrades back up - the same mechanism the game itself uses to restore upgrade
        // levels from a save file. Checking CurrentUpgradeLevel alone wouldn't catch a case where
        // that replay left the card's actual values (damage repeats, gem slots) stale, so compare
        // against a card that was genuinely upgraded once from scratch and never downgraded.
        var reference = (RefractedBeam)await ctx.AddCardToHand<RefractedBeam>();
        CardCmd.Upgrade(reference);

        Assert.AreEqual(reference.DynamicVars.Repeat.IntValue, card.DynamicVars.Repeat.IntValue,
            "The downgraded card's Repeat value should match a genuinely-once-upgraded card's, not be stale.");
        Assert.AreEqual(reference.GemSlots, card.GemSlots,
            "The downgraded card's GemSlots should match a genuinely-once-upgraded card's, not be stale.");
    }

    [CardTest(typeof(Guardian.GuardianCode.Core.Guardian))]
    public async Task DowngradingAStackedRefractedBeamDoesNotUnsocketItsGems(TestContext ctx)
    {
        // Gems live in BaseLib's own CardModifier list on the card instance, entirely separate
        // from the CurrentUpgradeLevel/DynamicVars/keywords state that DowngradeInternal (and our
        // patch's re-upgrade replay) touches, so downgrading should never unsocket them - confirm
        // that directly, and that a socketed gem still fires its effect (Strength) when played.
        var card = (RefractedBeam)await ctx.AddCardToHand<RefractedBeam>();
        var socketCard = (IGemSocketCard)card;
        CardCmd.Upgrade(card);
        CardCmd.Upgrade(card);
        socketCard.AddGem(CardModifier.Get<RubyGem>());
        Assert.AreEqual(1, socketCard.Gems.Count, "Sanity check: gem should be socketed before downgrading.");
        var gem = socketCard.Gems[0]; // AddGem clones the canonical modifier to make it mutable, so grab the real instance.

        CardCmd.Downgrade(card);

        Assert.AreEqual(1, socketCard.Gems.Count, "Downgrading should not unsocket an already-socketed gem.");
        Assert.IsTrue(socketCard.Gems.Contains(gem), "The same gem instance should still be socketed after downgrading.");

        var enemy = ctx.Combat.HittableEnemies.First();
        var strengthBefore = ctx.Player.Creature.GetInstancedPowerAmountSum<StrengthPower>();
        await ctx.PlayCard(card, enemy);
        var strengthAfter = ctx.Player.Creature.GetInstancedPowerAmountSum<StrengthPower>();
        Assert.IsTrue(strengthAfter > strengthBefore,
            "The socketed Ruby Gem should still grant Strength when the downgraded card is played.");
    }

    [CardTest(typeof(Guardian.GuardianCode.Core.Guardian))]
    public async Task DowngradeDeactivatesOverflowGemsButKeepsThemForWhenSlotsComeBack(TestContext ctx)
    {
        // A +2 Refracted Beam has 3 gem slots; downgrading to +1 shrinks that to 2. With all 3
        // slots filled, the 3rd gem shouldn't just keep firing invisibly (it's not shown in the
        // socket display, which only ever draws GemSlots-many icons) - but it also shouldn't be
        // deleted: re-upgrading back should bring it back to life with no extra bookkeeping,
        // since it's still physically socketed the whole time.
        var card = (RefractedBeam)await ctx.AddCardToHand<RefractedBeam>();
        var socketCard = (IGemSocketCard)card;
        CardCmd.Upgrade(card);
        CardCmd.Upgrade(card);
        Assert.AreEqual(3, socketCard.GemSlots, "Sanity: +2 Refracted Beam should have 3 gem slots.");
        socketCard.AddGem(CardModifier.Get<RubyGem>());
        socketCard.AddGem(CardModifier.Get<RubyGem>());
        socketCard.AddGem(CardModifier.Get<RubyGem>());
        Assert.AreEqual(3, socketCard.Gems.Count, "Sanity: 3 gems socketed into 3 slots.");

        CardCmd.Downgrade(card);
        Assert.AreEqual(2, socketCard.GemSlots, "Sanity: downgrading to +1 should shrink capacity to 2 slots.");
        Assert.AreEqual(3, socketCard.Gems.Count, "The overflow gem should stay socketed, not be removed.");

        var enemy = ctx.Combat.HittableEnemies.First();
        var strengthBeforeShrunk = ctx.Player.Creature.GetInstancedPowerAmountSum<StrengthPower>();
        await ctx.PlayCard(card, enemy);
        var strengthAfterShrunk = ctx.Player.Creature.GetInstancedPowerAmountSum<StrengthPower>();
        Assert.AreEqual(4, strengthAfterShrunk - strengthBeforeShrunk,
            "Only the 2 gems within the shrunk capacity should fire (2 Strength each), not the 3rd overflow gem.");

        CardCmd.Upgrade(card);
        Assert.AreEqual(3, socketCard.GemSlots, "Sanity: re-upgrading should restore the 3rd slot.");
        var strengthBeforeRestored = ctx.Player.Creature.GetInstancedPowerAmountSum<StrengthPower>();
        await ctx.PlayCard(card, enemy);
        var strengthAfterRestored = ctx.Player.Creature.GetInstancedPowerAmountSum<StrengthPower>();
        Assert.AreEqual(6, strengthAfterRestored - strengthBeforeRestored,
            "Once the slot is restored, the previously-overflowing gem should fire again too, with no re-socketing needed.");
    }

    [CardTest(typeof(Guardian.GuardianCode.Core.Guardian))]
    public async Task StandaloneGemCardStillPlaysItsOwnGemEffect(TestContext ctx)
    {
        // Reported bug: GemCard<T> reports GemSlots = 0 (only so its own overlay stays hidden -
        // it's not a real socket), but GemModel.OnPlay was reusing that same value as a capacity
        // check (SocketIndex >= GemSlots), which discarded the standalone card's own gem at
        // index 0 before it ever ran. A bare Ruby card socketed nothing and did nothing.
        var ruby = await ctx.AddCardToHand<Ruby>();
        var strengthBefore = ctx.Player.Creature.GetInstancedPowerAmountSum<StrengthPower>();

        await ctx.PlayCard(ruby);

        var strengthAfter = ctx.Player.Creature.GetInstancedPowerAmountSum<StrengthPower>();
        Assert.IsTrue(strengthAfter > strengthBefore,
            "Playing a standalone Ruby gem card should still grant Strength, not silently no-op.");
    }
}
