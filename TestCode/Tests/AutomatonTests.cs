using Automaton.AutomatonCode.Cards.Common;
using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.TestCode;

public class AutomatonTests
{
    // Regression guard: MergeConflictPower.AfterCardGeneratedForCombat fires again for the
    // clone it adds via AddGeneratedCardToCombat, which used to re-trigger itself and cascade
    // into Amount copies from a single Function creation instead of ticking down by 1 per
    // creation and making exactly one copy each time.
    [CardTest(typeof(Automaton.AutomatonCode.Core.Automaton))]
    public async Task MergeConflictOnlyMakesOneCopyPerFunctionCreation(TestContext ctx)
    {
        var choiceCtx = new BlockingPlayerChoiceContext();
        await PowerCmd.Apply<MergeConflictPower>(choiceCtx, ctx.Player.Creature, 2, ctx.Player.Creature, null);

        for (var i = 0; i < AutomatonCmd.GetMax(ctx.Player); i++)
            await AutomatonCmd.EncodeCard<OilSpill>(ctx.Player, choiceCtx);

        var functionCount = ctx.Player.Hand.Count(c => c is FunctionCard);
        Assert.AreEqual(2, functionCount,
            "One Function creation with MergeConflict at 2 should yield exactly one extra copy (2 total), not cascade into more.");

        var remaining = ctx.Player.Creature.GetInstancedPowerAmountSum<MergeConflictPower>();
        Assert.AreEqual(1, remaining, "MergeConflict should tick down by exactly 1 per Function creation.");
    }
}
