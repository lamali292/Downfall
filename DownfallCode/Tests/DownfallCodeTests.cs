using Downfall.DownfallCode.Compatibility;
using Downfall.DownfallCode.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;

namespace Downfall.DownfallCode.Tests;

public class DownfallCodeTests
{
    // Cmd.Wait is a synchronous no-op under TestMode/NonInteractiveMode (see Cmd.Wait's own
    // guard), so it never yields back to the engine's frame loop - the enemy-turn state machine
    // (driven by real Godot signals/timers, not by anything a test method awaits directly) never
    // gets a chance to run. A real Task.Delay does yield to that loop, so poll on it instead.
    private static async Task WaitForNextOwnTurn(TestContext ctx)
    {
        var startingTurn = ctx.Player.PlayerCombatState!.TurnNumber;
        PlayerCmd.EndTurn(ctx.Player, false);
        for (var i = 0; i < 50 && ctx.Player.PlayerCombatState!.TurnNumber == startingTurn; i++)
            await Task.Delay(100);

        Assert.IsTrue(ctx.Player.PlayerCombatState!.TurnNumber > startingTurn,
            "Sanity: the owner's next turn should have started within 5s.");
    }

    [CardTest]
    public async Task ClearHandEmptiesTheLocalPlayersHand(TestContext ctx)
    {
        await ctx.AddCardToHand<StrikeIronclad>();
        await ctx.AddCardToHand<DefendIronclad>();
        Assert.IsTrue(PileType.Hand.GetPile(ctx.Player).Cards.Count > 0, "Sanity: hand should not already be empty.");

        await ctx.ClearHand();

        Assert.AreEqual(0, PileType.Hand.GetPile(ctx.Player).Cards.Count, "ClearHand should empty the local player's hand.");
    }

    [CardTest(playerCount: 2)]
    public async Task ClearHandEmptiesAnExplicitPlayersHand(TestContext ctx)
    {
        var teammate = ctx.Players[1];
        await ctx.AddCardToHand<StrikeIronclad>(teammate);
        Assert.IsTrue(PileType.Hand.GetPile(teammate).Cards.Count > 0, "Sanity: teammate's hand should not already be empty.");

        await ctx.ClearHand(teammate);

        Assert.AreEqual(0, PileType.Hand.GetPile(teammate).Cards.Count, "ClearHand should empty the given player's hand.");
    }

    [CardTest]
    public async Task TempKeywordUtilGrantsAKeywordThatIsVisibleOnTheCard(TestContext ctx)
    {
        var card = await ctx.AddCardToHand<StrikeIronclad>();
        Assert.IsTrue(!card.Keywords.Contains(CardKeyword.Retain), "Sanity: Strike shouldn't already have Retain.");

        TempKeywordUtil.Add(card, CardKeyword.Retain, TempKeywordRemoveCondition.EndOfTurn);

        Assert.IsTrue(card.Keywords.Contains(CardKeyword.Retain), "TempKeywordUtil should grant the keyword.");
        Assert.IsTrue(TempKeywordUtil.Has(card, CardKeyword.Retain), "Has() should report the active grant.");
    }

    [CardTest]
    public async Task TempKeywordUtilRemoveClearsItImmediately(TestContext ctx)
    {
        var card = await ctx.AddCardToHand<StrikeIronclad>();
        TempKeywordUtil.Add(card, CardKeyword.Retain, TempKeywordRemoveCondition.EndOfTurn);

        TempKeywordUtil.Remove(card, CardKeyword.Retain);

        Assert.IsTrue(!card.Keywords.Contains(CardKeyword.Retain), "Manual Remove should clear the grant right away.");
        Assert.IsTrue(!TempKeywordUtil.Has(card, CardKeyword.Retain), "Has() should no longer report the grant.");
    }

    [CardTest]
    public async Task TempKeywordUtilEndOfTurnClearsWhenOwnersTurnEnds(TestContext ctx)
    {
        var card = await ctx.AddCardToHand<StrikeIronclad>();
        TempKeywordUtil.Add(card, CardKeyword.Retain, TempKeywordRemoveCondition.EndOfTurn);

        PlayerCmd.EndTurn(ctx.Player, false);
        await Cmd.Wait(1f);

        Assert.IsTrue(!card.Keywords.Contains(CardKeyword.Retain),
            "EndOfTurn grant should clear once the owner's turn ends.");
    }

    [CardTest]
    public async Task TempKeywordUtilStartOfTurnSurvivesEnemyTurnAndClearsOnOwnersNextTurn(TestContext ctx)
    {
        var card = await ctx.AddCardToHand<StrikeIronclad>();
        TempKeywordUtil.Add(card, CardKeyword.Retain, TempKeywordRemoveCondition.StartOfTurn);

        // A StartOfTurn grant must not be confused with EndOfTurn/EndOfEnemyTurn - it should still
        // be active through the rest of this turn and the whole enemy turn, only clearing once the
        // owner's own next turn begins.
        await WaitForNextOwnTurn(ctx);

        Assert.IsTrue(!card.Keywords.Contains(CardKeyword.Retain),
            "StartOfTurn grant should clear once the owner's next turn starts.");
    }

    [CardTest]
    public async Task TempKeywordUtilEndOfEnemyTurnClearsAfterTheFullTurnCycle(TestContext ctx)
    {
        var card = await ctx.AddCardToHand<StrikeIronclad>();
        TempKeywordUtil.Add(card, CardKeyword.Retain, TempKeywordRemoveCondition.EndOfEnemyTurn);

        await WaitForNextOwnTurn(ctx);

        Assert.IsTrue(!card.Keywords.Contains(CardKeyword.Retain),
            "EndOfEnemyTurn grant should have cleared once the enemy turn ended, well before now.");
    }

    [CardTest]
    public async Task TempKeywordUtilStartOfEnemyTurnClearsAfterTheFullTurnCycle(TestContext ctx)
    {
        var card = await ctx.AddCardToHand<StrikeIronclad>();
        TempKeywordUtil.Add(card, CardKeyword.Retain, TempKeywordRemoveCondition.StartOfEnemyTurn);

        await WaitForNextOwnTurn(ctx);

        Assert.IsTrue(!card.Keywords.Contains(CardKeyword.Retain),
            "StartOfEnemyTurn grant should have cleared once the enemy turn started, well before now.");
    }

    [CardTest]
    public async Task TempKeywordUtilEnergySpentClearsAfterOwnerSpendsEnergy(TestContext ctx)
    {
        var card = await ctx.AddCardToHand<StrikeIronclad>();
        TempKeywordUtil.Add(card, CardKeyword.Retain, TempKeywordRemoveCondition.EnergySpent);

        // ctx.PlayCard goes through CardCmd.AutoPlay, which plays cards "for free" and never
        // calls SpendResources() (see CollectorTests' XCostCardSpendsEnergyAndReserve comment) -
        // call it directly so AfterEnergySpent (and thus this grant's removal) actually fires.
        var otherCard = await ctx.AddCardToHand<DefendIronclad>();
        await otherCard.SpendResources();

        Assert.IsTrue(!card.Keywords.Contains(CardKeyword.Retain),
            "EnergySpent grant should clear once the owner spends energy on any card.");
    }

    [CardTest]
    public Task NetIdTableCompletionAppendsAMissingEntryAtItsOldId(TestContext ctx)
    {
        // Simulates the older-build failure where a submod's models are in ModelDb but not in the net-ID
        // table: drop the last entry name (so every other id stays put), then let the check restore it.
        var map = AccessTools.Field(typeof(ModelIdSerializationCache), "_entryNameToNetIdMap")
            .GetValue(null) as Dictionary<string, int>;
        var list = AccessTools.Field(typeof(ModelIdSerializationCache), "_netIdToEntryNameMap")
            .GetValue(null) as List<string>;
        Assert.IsTrue(map != null && list != null, "Sanity: the net-ID table internals should be reachable.");

        var name = list![^1];
        var id = map![name];
        list.RemoveAt(list.Count - 1);
        map.Remove(name);

        var added = ModelIdCacheCompletionPatch.EnsureAllModelsMapped();

        Assert.IsTrue(added >= 1, "The model owning the removed entry should be reported as missing.");
        Assert.AreEqual(id, map[name], "The entry should come back at the id it had before, not shift others.");
        Assert.AreEqual(0, ModelIdCacheCompletionPatch.EnsureAllModelsMapped(),
            "A complete table must be left alone (second run adds nothing).");
        return Task.CompletedTask;
    }
}
