using Downfall.DownfallCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Downfall.TestCode;

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
}
