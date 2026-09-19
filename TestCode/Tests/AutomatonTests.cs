using Automaton.AutomatonCode.Cards.Common;
using Automaton.AutomatonCode.Cards.Rare;
using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.Cards.Uncommon;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Powers;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
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

    // Regression guard: Full Release (PowerEncode) makes a Function skip its other Encodings on
    // play entirely (FunctionCard.OnPlay breaks right after PowerEncode) and instead release them
    // later through FullReleasePower, whose turn-start trigger passes no CardModel source and no
    // fixed target (the target enemy is re-rolled each turn). Class Default must NOT scale that
    // deferred Block/Damage: baking a snapshot in at grant time would also freeze in
    // target-dependent modifiers (Weak, Vulnerable, ...) against whatever enemy happened to be
    // resolved first, which is wrong once the power re-targets on later turns. Class Default only
    // affects Attack/Skill Functions (CardType.Power Functions never resolve Block/Damage through
    // a play at all - see FunctionCard.OnPlay), so a compiled Power Function's preview must not
    // show the bonus either, matching what actually happens when it's played.
    [CardTest(typeof(Automaton.AutomatonCode.Core.Automaton))]
    public async Task ClassDefaultDoesNotScaleFullReleaseDeferredBlock(TestContext ctx)
    {
        var choiceCtx = new BlockingPlayerChoiceContext();
        var classDefault =
            await PowerCmd.Apply<ClassDefaultPower>(choiceCtx, ctx.Player.Creature, 5, ctx.Player.Creature, null);

        // BronzeCore auto-encodes a Defend + Strike on turn 1 (AutomatonCode/Relics/BronzeCore.cs).
        // Play one filler Boost to complete and flush that batch before setting up the real one.
        await ctx.PlayCard(await ctx.AddCardToHand<Boost>());

        // Boost (Block + Strength) x2 and Full Release (Power) x1 compile into a fresh 3-card
        // Function that carries both Block and the Full Release Power effect.
        await ctx.PlayCard(await ctx.AddCardToHand<Boost>());
        await ctx.PlayCard(await ctx.AddCardToHand<Boost>());
        await ctx.PlayCard(await ctx.AddCardToHand<FullRelease>());

        var function = ctx.Player.Hand.OfType<FunctionCard>()
            .FirstOrDefault(f => f.DynamicVars.Power<FullReleasePower>().BaseValue > 0);
        Assert.IsTrue(function != null, "Boost x2 + Full Release should have compiled into a Function in hand.");
        Assert.IsTrue(function!.Type == CardType.Power, "A Function carrying Full Release should be CardType.Power.");
        var rawBlock = function.DynamicVars.Block.BaseValue;
        Assert.IsTrue(rawBlock > 0, "Function should carry the encoded Block from Boost.");

        var previewBonus = classDefault!.ModifyBlockAdditive(ctx.Player.Creature, rawBlock,
            function.DynamicVars.Block.Props, function, null);
        Assert.AreEqual(0m, previewBonus,
            "Class Default must not preview a Block bonus on a Power Function - it will never actually be delivered.");

        await ctx.PlayCard(function);

        var granted = ctx.Player.Creature.GetPowerInstances<FullReleasePower>().LastOrDefault();
        Assert.IsTrue(granted != null, "Playing the Function should grant a Full Release Power instance.");
        Assert.AreEqual(rawBlock, granted!.DynamicVars.Block.BaseValue,
            "Class Default must not scale the Block deferred through Full Release Power - only Block from a Function card actually being played.");
    }
}
