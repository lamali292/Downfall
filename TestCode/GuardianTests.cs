using Guardian.GuardianCode.Cards.Basic;
using Guardian.GuardianCode.Cards.Common;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.Relics;
using MegaCrit.Sts2.Core.AutoSlay;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

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
}
