using Automaton.AutomatonCode.Cards.Common;
using Automaton.AutomatonCode.Cards.Rare;
using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.Cards.Uncommon;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Extensions;
using Automaton.AutomatonCode.Powers;
using Automaton.AutomatonCode.Relics;
using BaseLib.Extensions;
using Downfall.DownfallCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.TestCode;

/// <summary>
///     Characterization tests for how a Function is assembled from its source cards (values, type,
///     target, play order, position-dependent effects, compile effects, title, description lines).
///     They pin today's behavior so the Encode/Compile keyword refactor
///     (.scratch/automaton-encode-compile-keywords) can be proven to change nothing. Expected values
///     are read off the source cards' own dynamic vars instead of hardcoded, so balance changes do not
///     break them.
/// </summary>
public class AutomatonFunctionTests
{
    private static T Make<T>(TestContext ctx) where T : CardModel
    {
        return ctx.Combat.CreateCard<T>(ctx.Player);
    }

    /// Empties the Encode pile (BronzeCore leaves cards in it) so the next batch forms exactly one Function.
    private static async Task FlushEncodePile(TestContext ctx)
    {
        var choiceCtx = new BlockingPlayerChoiceContext();
        while (ctx.Player.EncodePile.Count > 0)
            await AutomatonCmd.EncodeCard<OilSpill>(ctx.Player, choiceCtx);
    }

    /// Encodes the given cards in order into an empty Encode pile and returns the Function they compile into.
    private static async Task<FunctionCard> Compile(TestContext ctx, params CardModel[] sources)
    {
        Assert.AreEqual(AutomatonCmd.GetMax(ctx.Player), sources.Length,
            "Setup: the number of source cards must fill the Encode pile exactly.");
        await FlushEncodePile(ctx);
        var choiceCtx = new BlockingPlayerChoiceContext();
        FunctionCard? function = null;
        foreach (var source in sources)
            function = await AutomatonCmd.EncodeCard(source, choiceCtx) ?? function;
        Assert.IsTrue(function != null, "Encoding a full pile should have produced a Function.");
        return function!;
    }

    // Encode values are summed across the source cards, per effect.
    [CardTest(typeof(Automaton.AutomatonCode.Core.Automaton))]
    public async Task EncodedValuesAreSummedAcrossSources(TestContext ctx)
    {
        var boost = Make<Boost>(ctx);
        var oilSpill = Make<OilSpill>(ctx);
        var fragment = Make<Fragment>(ctx);
        var function = await Compile(ctx, boost, oilSpill, fragment);

        Assert.AreEqual(boost.DynamicVars.Block.BaseValue + fragment.DynamicVars.Block.BaseValue,
            function.DynamicVars.Block.BaseValue, "Block should be Boost + Fragment.");
        Assert.AreEqual(oilSpill.DynamicVars.Damage.BaseValue + fragment.DynamicVars.Damage.BaseValue,
            function.DynamicVars.Damage.BaseValue, "Damage should be Oil Spill + Fragment.");
        Assert.AreEqual(oilSpill.DynamicVars.Poison.BaseValue, function.DynamicVars.Poison.BaseValue,
            "Poison should come from Oil Spill only.");
    }

    [CardTest(typeof(Automaton.AutomatonCode.Core.Automaton))]
    public async Task DebuffSoulburnEnergyAndDazedEncodesMergeOntoFunction(TestContext ctx)
    {
        var deprecate = Make<Deprecate>(ctx);
        var invalidate = Make<Invalidate>(ctx);
        var explode = Make<Explode>(ctx);
        var debuffs = await Compile(ctx, deprecate, invalidate, explode);
        Assert.AreEqual(deprecate.DynamicVars.Weak.BaseValue, debuffs.DynamicVars.Weak.BaseValue, "Weak.");
        Assert.AreEqual(invalidate.DynamicVars.Vulnerable.BaseValue, debuffs.DynamicVars.Vulnerable.BaseValue,
            "Vulnerable.");
        Assert.AreEqual(explode.DynamicVars.Power<SoulBurnPower>().BaseValue,
            debuffs.DynamicVars.Power<SoulBurnPower>().BaseValue, "Soulburn.");

        var buggyMess = Make<BuggyMess>(ctx);
        var energy = await Compile(ctx, buggyMess, Make<Boost>(ctx), Make<Boost>(ctx));
        Assert.AreEqual(buggyMess.DynamicVars.Energy.BaseValue, energy.DynamicVars.Energy.BaseValue, "Energy.");
        Assert.AreEqual(buggyMess.DynamicVars["Dazed"].BaseValue, energy.DynamicVars["Dazed"].BaseValue, "Dazed.");
    }

    // Type folds Power > Attack > Skill; target folds Power => Self, else AnyEnemy > AllEnemies > Self.
    [CardTest(typeof(Automaton.AutomatonCode.Core.Automaton))]
    public async Task FunctionTypeAndTargetFoldFromEncodedEffects(TestContext ctx)
    {
        var block = await Compile(ctx, Make<Boost>(ctx), Make<Boost>(ctx), Make<Boost>(ctx));
        Assert.IsTrue(block.Type == CardType.Skill && block.TargetType == TargetType.Self,
            "Block-only Function should be a Skill targeting Self.");

        var soulburn = await Compile(ctx, Make<Boost>(ctx), Make<Explode>(ctx), Make<Boost>(ctx));
        Assert.IsTrue(soulburn.Type == CardType.Skill && soulburn.TargetType == TargetType.AllEnemies,
            "Block + Soulburn should be a Skill targeting AllEnemies.");

        var debuff = await Compile(ctx, Make<Boost>(ctx), Make<Explode>(ctx), Make<Deprecate>(ctx));
        Assert.IsTrue(debuff.Type == CardType.Skill && debuff.TargetType == TargetType.AnyEnemy,
            "AnyEnemy must win over AllEnemies and Self.");

        var attack = await Compile(ctx, Make<Boost>(ctx), Make<OilSpill>(ctx), Make<Boost>(ctx));
        Assert.IsTrue(attack.Type == CardType.Attack && attack.TargetType == TargetType.AnyEnemy,
            "Damage makes the Function an Attack targeting AnyEnemy.");

        var power = await Compile(ctx, Make<Boost>(ctx), Make<FullRelease>(ctx), Make<OilSpill>(ctx));
        Assert.IsTrue(power.Type == CardType.Power && power.TargetType == TargetType.Self,
            "Full Release makes the Function a Power and forces Self, even with Damage present.");
    }

    // Encode effects fire in a fixed per-effect order, not in card order: Damage resolves before
    // Vulnerable, so the Function's own Vulnerable never boosts its own Damage.
    [CardTest(typeof(Automaton.AutomatonCode.Core.Automaton))]
    public async Task PlayOrderIsFixedPerEffect(TestContext ctx)
    {
        var invalidate = Make<Invalidate>(ctx);
        var fragment = Make<Fragment>(ctx);
        var function = await Compile(ctx, invalidate, fragment, Make<Deprecate>(ctx));
        var enemy = ctx.Combat.HittableEnemies.First();
        var hpBefore = (decimal)enemy.CurrentHp;
        var blockBefore = (decimal)ctx.Player.Creature.Block;

        await ctx.PlayCard(function, enemy);

        Assert.AreEqual(hpBefore - function.DynamicVars.Damage.BaseValue, (decimal)enemy.CurrentHp,
            "Damage must resolve before Vulnerable is applied by the same Function.");
        Assert.IsTrue(enemy.GetInstancedPowerAmountSum<VulnerablePower>() > 0, "Vulnerable should be applied.");
        Assert.IsTrue(enemy.GetInstancedPowerAmountSum<WeakPower>() > 0, "Weak should be applied.");
        Assert.AreEqual(blockBefore + fragment.DynamicVars.Block.BaseValue, (decimal)ctx.Player.Creature.Block,
            "Block should be gained.");
    }

    // Full Release (PowerEncode) ends the sequence: nothing else in the Function resolves on play.
    [CardTest(typeof(Automaton.AutomatonCode.Core.Automaton))]
    public async Task PowerEncodeEndsTheSequence(TestContext ctx)
    {
        var function = await Compile(ctx, Make<Boost>(ctx), Make<FullRelease>(ctx), Make<OilSpill>(ctx));
        var enemy = ctx.Combat.HittableEnemies.First();
        var hpBefore = (decimal)enemy.CurrentHp;
        var blockBefore = (decimal)ctx.Player.Creature.Block;

        await ctx.PlayCard(function);

        Assert.AreEqual(hpBefore, (decimal)enemy.CurrentHp, "Damage is deferred through Full Release, not dealt on play.");
        Assert.AreEqual(blockBefore, (decimal)ctx.Player.Creature.Block,
            "Block is deferred through Full Release, not gained on play.");
        Assert.IsTrue(ctx.Player.Creature.GetPowerInstances<FullReleasePower>().Any(),
            "Playing the Function should grant Full Release.");
    }

    // Enchantments on a source card are folded into the merged value (Block and Damage only).
    [CardTest(typeof(Automaton.AutomatonCode.Core.Automaton))]
    public async Task EnchantedBlockAndDamageAreMerged(TestContext ctx)
    {
        var frontload = Make<Frontload>(ctx);
        CardCmd.Enchant<Nimble>(frontload, 2);
        var oilSpill = Make<OilSpill>(ctx);
        CardCmd.Enchant<Sharp>(oilSpill, 3);
        var function = await Compile(ctx, frontload, oilSpill, Make<Boost>(ctx));

        var plainBoost = Make<Boost>(ctx);
        Assert.AreEqual(frontload.DynamicVars.Block.BaseValue + 2 + plainBoost.DynamicVars.Block.BaseValue,
            function.DynamicVars.Block.BaseValue, "Nimble's +2 Block should be merged.");
        Assert.AreEqual(oilSpill.DynamicVars.Damage.BaseValue + 3, function.DynamicVars.Damage.BaseValue,
            "Sharp's +3 Damage should be merged.");
    }

    // Constructor only adds its extra Block at Start, Separator only adds its extra Damage in the
    // Middle, Terminator only adds a replay at the End.
    [CardTest(typeof(Automaton.AutomatonCode.Core.Automaton))]
    public async Task PositionDependentEffectsApplyOnlyAtTheirPosition(TestContext ctx)
    {
        var constructor = Make<Constructor>(ctx);
        var separator = Make<Separator>(ctx);
        var terminator = Make<Terminator>(ctx);
        var inPlace = await Compile(ctx, constructor, separator, terminator);
        Assert.AreEqual(constructor.DynamicVars.Block.BaseValue + constructor.DynamicVars["ExtraBlock"].BaseValue,
            inPlace.DynamicVars.Block.BaseValue, "Constructor at Start adds its extra Block.");
        Assert.AreEqual(separator.DynamicVars.Damage.BaseValue + separator.DynamicVars["ExtraDamage"].BaseValue,
            inPlace.DynamicVars.Damage.BaseValue, "Separator in the Middle adds its extra Damage.");
        Assert.AreEqual(1, inPlace.BaseReplayCount, "Terminator at End adds one replay.");

        var boost = Make<Boost>(ctx);
        var misplacedConstructor = Make<Constructor>(ctx);
        var misplacedSeparator = Make<Separator>(ctx);
        var misplaced = await Compile(ctx, boost, misplacedConstructor, misplacedSeparator);
        Assert.AreEqual(boost.DynamicVars.Block.BaseValue + misplacedConstructor.DynamicVars.Block.BaseValue,
            misplaced.DynamicVars.Block.BaseValue, "Constructor outside Start adds no extra Block.");
        Assert.AreEqual(misplacedSeparator.DynamicVars.Damage.BaseValue, misplaced.DynamicVars.Damage.BaseValue,
            "Separator at End adds no extra Damage.");
        Assert.AreEqual(0, misplaced.BaseReplayCount, "No Terminator at End means no replay.");
    }

    // Card-level changes to the Function: Frontload's Retain and Null Pointer's fixed cost.
    [CardTest(typeof(Automaton.AutomatonCode.Core.Automaton))]
    public async Task CardLevelFunctionChangesAreApplied(TestContext ctx)
    {
        var retain = await Compile(ctx, Make<Frontload>(ctx), Make<Boost>(ctx), Make<Boost>(ctx));
        Assert.IsTrue(retain.Keywords.Contains(CardKeyword.Retain), "Frontload should make the Function Retain.");

        var plain = await Compile(ctx, Make<Boost>(ctx), Make<Boost>(ctx), Make<Boost>(ctx));
        Assert.IsTrue(!plain.Keywords.Contains(CardKeyword.Retain), "A Function without Frontload should not Retain.");

        var nullPointer = Make<NullPointer>(ctx);
        var costed = await Compile(ctx, Make<Boost>(ctx), nullPointer, Make<Boost>(ctx));
        Assert.AreEqual(nullPointer.DynamicVars.Energy.IntValue, costed.EnergyCost.GetResolved(),
            "Null Pointer should set the Function's cost to its Energy value.");
    }

    // Compile effects: OnCompile fires exactly once when the Function is created (not on play), and
    // the merged value equals the source card's own var.
    [CardTest(typeof(Automaton.AutomatonCode.Core.Automaton))]
    public async Task CompileEffectsFireOnceAtCompileTime(TestContext ctx)
    {
        await FlushEncodePile(ctx);
        var boost = Make<Boost>(ctx);
        var spike = Make<Spike>(ctx);
        decimal strengthBefore = ctx.Player.Creature.GetInstancedPowerAmountSum<StrengthPower>();
        decimal thornsBefore = ctx.Player.Creature.GetInstancedPowerAmountSum<ThornsPower>();
        var stashBefore = ctx.Player.StashPile.Count;

        var function = await Compile(ctx, boost, spike, Make<OilSpill>(ctx));

        Assert.AreEqual(strengthBefore + boost.DynamicVars.Power<StrengthPower>().BaseValue,
            (decimal)ctx.Player.Creature.GetInstancedPowerAmountSum<StrengthPower>(), "Strength applied once at compile.");
        Assert.AreEqual(thornsBefore + spike.DynamicVars.Power<ThornsPower>().BaseValue,
            (decimal)ctx.Player.Creature.GetInstancedPowerAmountSum<ThornsPower>(), "Thorns applied once at compile.");
        Assert.AreEqual(stashBefore + 1, ctx.Player.StashPile.Count, "Oil Spill stashes exactly one Error at compile.");

        int strengthAfterCompile = ctx.Player.Creature.GetInstancedPowerAmountSum<StrengthPower>();
        await ctx.PlayCard(function, ctx.Combat.HittableEnemies.First());
        Assert.AreEqual(strengthAfterCompile, ctx.Player.Creature.GetInstancedPowerAmountSum<StrengthPower>(),
            "Playing the Function must not re-fire compile effects.");
    }

    [CardTest(typeof(Automaton.AutomatonCode.Core.Automaton))]
    public async Task PerfectionTitleForConstructorSeparatorTerminator(TestContext ctx)
    {
        var perfection = new LocString("encode", "AUTOMATON-PERFECTION.functionName").GetFormattedText();

        var three = await Compile(ctx, Make<Constructor>(ctx), Make<Separator>(ctx), Make<Terminator>(ctx));
        Assert.AreEqual(perfection, three.Title, "Constructor, Separator, Terminator should be named Perfection.");

        var ordinary = await Compile(ctx, Make<Boost>(ctx), Make<Boost>(ctx), Make<Boost>(ctx));
        Assert.IsTrue(ordinary.Title != perfection, "An ordinary Function must not be named Perfection.");

        await RelicCmd.Obtain<ElectromagneticCoil>(ctx.Player);
        var separatorA = Make<Separator>(ctx);
        var separatorB = Make<Separator>(ctx);
        var four = await Compile(ctx, Make<Constructor>(ctx), separatorA, separatorB, Make<Terminator>(ctx));
        Assert.AreEqual(perfection, four.Title, "The 4-slot Perfection variant should also be named Perfection.");
        Assert.AreEqual(2 * (separatorA.DynamicVars.Damage.BaseValue + separatorA.DynamicVars["ExtraDamage"].BaseValue),
            four.DynamicVars.Damage.BaseValue, "Both middle Separators add their extra Damage in a 4-slot Function.");
    }

    // The Function's text is one line per active encode effect and per compile effect.
    [CardTest(typeof(Automaton.AutomatonCode.Core.Automaton))]
    public async Task DescriptionLinesListEveryActiveEffect(TestContext ctx)
    {
        var function = await Compile(ctx, Make<Boost>(ctx), Make<OilSpill>(ctx), Make<Fragment>(ctx));

        Assert.AreEqual(3, function.GetEncodeLines().Count(), "Block, Damage and Poison should each get a line.");
        Assert.AreEqual(2, function.GetCompileLines().Count(),
            "Compile Strength and Compile Error-to-Stash should each get a line.");
        Assert.IsTrue(function.GetEncodeLines().All(l => !string.IsNullOrWhiteSpace(l)), "No empty encode lines.");
        Assert.IsTrue(function.GetCompileLines().All(l => !string.IsNullOrWhiteSpace(l)), "No empty compile lines.");
    }
}
