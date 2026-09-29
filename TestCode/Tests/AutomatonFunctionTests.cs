using Automaton.AutomatonCode.Cards;
using Automaton.AutomatonCode.Cards.Basic;
using Automaton.AutomatonCode.Cards.Common;
using Automaton.AutomatonCode.Cards.Rare;
using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.Cards.Uncommon;
using Automaton.AutomatonCode.Compile;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using Automaton.AutomatonCode.Encode;
using Automaton.AutomatonCode.Extensions;
using Automaton.AutomatonCode.Functions;
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

        Assert.AreEqual(3, function.GetLines(AutomatonKeyword.Encode).Count(), "Block, Damage and Poison should each get a line.");
        Assert.AreEqual(2, function.GetLines(AutomatonKeyword.Compile).Count(),
            "Compile Strength and Compile Error-to-Stash should each get a line.");
        Assert.IsTrue(function.GetLines(AutomatonKeyword.Encode).All(l => !string.IsNullOrWhiteSpace(l)), "No empty encode lines.");
        Assert.IsTrue(function.GetLines(AutomatonKeyword.Compile).All(l => !string.IsNullOrWhiteSpace(l)), "No empty compile lines.");
    }

    // Encode effects play in ascending Order and Compile effects are listed in ascending Order; Order is
    // unique so neither is ever ambiguous.
    [CardTest(typeof(Automaton.AutomatonCode.Core.Automaton))]
    public Task EffectsAreSortedByUniqueOrder(TestContext ctx)
    {
        var orders = EffectRegistry.ValueEncodes.Select(e => e.Order).ToList();
        Assert.IsTrue(orders.SequenceEqual(orders.OrderBy(o => o)), "ValueEncodes must be sorted by Order.");
        Assert.AreEqual(orders.Count, orders.Distinct().Count(), "Encode effect Orders must be unique.");

        var compileOrders = EffectRegistry.Compilables.Select(c => c.Order).ToList();
        Assert.IsTrue(compileOrders.SequenceEqual(compileOrders.OrderBy(o => o)), "Compilables must be sorted by Order.");
        Assert.AreEqual(compileOrders.Count, compileOrders.Distinct().Count(), "Compile effect Orders must be unique.");
        return Task.CompletedTask;
    }

    // Every effect's explicit Id still resolves to its entry in encode.json (loc must not depend on class names).
    [CardTest(typeof(Automaton.AutomatonCode.Core.Automaton))]
    public Task EveryEffectHasItsLocEntry(TestContext ctx)
    {
        foreach (var e in EffectRegistry.ValueEncodes)
            Assert.IsTrue(new LocString("encode", e.GetType().GetPrefix() + e.Id + ".encode").Exists(),
                $"{e.GetType().Name}: missing encode.json entry for Id '{e.Id}'.");
        foreach (var e in new Encodable[] { new PowerEncode(), new RetainEncode(), new FunctionCostEncode() })
            Assert.IsTrue(new LocString("encode", e.GetType().GetPrefix() + e.Id + ".compile").Exists(),
                $"{e.GetType().Name}: missing encode.json '.compile' note for Id '{e.Id}'.");
        foreach (var c in EffectRegistry.Compilables)
            Assert.IsTrue(new LocString("encode", c.GetType().GetPrefix() + c.Id + ".compile").Exists(),
                $"{c.GetType().Name}: missing encode.json entry for Id '{c.Id}'.");
        return Task.CompletedTask;
    }

    // The registry, filled by the Automaton's mod initializer, holds exactly the public concrete effect
    // classes of this assembly - so adding an effect needs only the class, its loc entry and WithEncode /
    // WithCompile on a card, never a list edit.
    [CardTest(typeof(Automaton.AutomatonCode.Core.Automaton))]
    public Task RegistryHoldsEveryPublicEffect(TestContext ctx)
    {
        var types = typeof(Encodable).Assembly.GetTypes()
            .Where(t => t is { IsPublic: true, IsAbstract: false, IsGenericTypeDefinition: false } &&
                        t.GetConstructor(Type.EmptyTypes) != null);
        Assert.IsTrue(
            types.Where(t => t.IsSubclassOf(typeof(Encodable))).ToHashSet()
                .SetEquals(EffectRegistry.Encodables.Select(e => e.GetType())),
            "Registry Encode effects must match the public Encodable classes.");
        Assert.IsTrue(
            types.Where(t => t.IsSubclassOf(typeof(Compilable))).ToHashSet()
                .SetEquals(EffectRegistry.Compilables.Select(c => c.GetType())),
            "Registry Compile effects must match the public Compilable classes.");
        return Task.CompletedTask;
    }

    // The registry is frozen once read (the Function card has taken its vars from it), so a late
    // registration fails loudly instead of being silently missing from the Function; re-registering
    // something already known, or scanning the assembly again, is harmless.
    [CardTest(typeof(Automaton.AutomatonCode.Core.Automaton))]
    public Task RegistryIsFrozenAndIdempotent(TestContext ctx)
    {
        var encodeCount = EffectRegistry.Encodables.Count;
        var compileCount = EffectRegistry.Compilables.Count;

        EffectRegistry.Register(new BlockEncode());
        EffectRegistry.Register(new StrengthCompile());
        EffectRegistry.RegisterAssembly(typeof(Encodable).Assembly);
        Assert.AreEqual(encodeCount, EffectRegistry.Encodables.Count, "Known Encode effects must not be added twice.");
        Assert.AreEqual(compileCount, EffectRegistry.Compilables.Count, "Known Compile effects must not be added twice.");

        var threw = false;
        try
        {
            EffectRegistry.Register(new LateTestEncode());
        }
        catch (InvalidOperationException)
        {
            threw = true;
        }

        Assert.IsTrue(threw, "Registering a new effect after the registry was read must throw.");
        Assert.AreEqual(encodeCount, EffectRegistry.Encodables.Count, "A rejected registration must not change the registry.");
        return Task.CompletedTask;
    }

    /// Private on purpose: the assembly scan only takes public classes, so this never enters the registry.
    private sealed class LateTestEncode : Encodable
    {
        public override string Id => "LATE_TEST_ENCODE";
    }

    // A cloned Function (e.g. by Merge Conflict) has its own cloned vars; its type, target, text and play
    // behavior must come from those, not from the original.
    [CardTest(typeof(Automaton.AutomatonCode.Core.Automaton))]
    public async Task ClonedFunctionKeepsItsBehavior(TestContext ctx)
    {
        // No Boost here: its compile Strength would add to the damage the clone deals.
        var function = await Compile(ctx, Make<Deprecate>(ctx), Make<OilSpill>(ctx), Make<Fragment>(ctx));
        var clone = (FunctionCard)function.CreateClone();

        Assert.IsTrue(clone.DynamicVars != function.DynamicVars, "Setup: a clone should own its vars.");
        Assert.IsTrue(clone.Type == function.Type && clone.TargetType == function.TargetType,
            "A clone keeps the original's type and target.");
        Assert.AreEqual(function.GetLines(AutomatonKeyword.Encode).Count(), clone.GetLines(AutomatonKeyword.Encode).Count(),
            "A clone lists the same encode lines.");

        clone.DynamicVars.Damage.BaseValue += 10;
        Assert.IsTrue(function.DynamicVars.Damage.BaseValue != clone.DynamicVars.Damage.BaseValue,
            "Setup: changing the clone's var must not change the original's.");

        await CardPileCmd.AddGeneratedCardToCombat(clone, PileType.Hand, ctx.Player);
        var enemy = ctx.Combat.HittableEnemies.First();
        var hpBefore = (decimal)enemy.CurrentHp;
        await ctx.PlayCard(clone, enemy);
        Assert.AreEqual(hpBefore - clone.DynamicVars.Damage.BaseValue, (decimal)enemy.CurrentHp,
            "The clone deals its own (changed) damage when played.");
    }

    // What an effect changes about the Function itself (Retain, a fixed cost, Power) is listed in the
    // Function's Compile lines, one per effect, in card order.
    [CardTest(typeof(Automaton.AutomatonCode.Core.Automaton))]
    public async Task FunctionNotesAreListedAsCompileLines(TestContext ctx)
    {
        var withNotes = await Compile(ctx, Make<Frontload>(ctx), Make<NullPointer>(ctx), Make<Deprecate>(ctx));
        Assert.AreEqual(2, withNotes.GetLines(AutomatonKeyword.Compile).Count(),
            "Frontload (Retain) and Null Pointer (cost) each add one Compile line.");

        var power = await Compile(ctx, Make<FullRelease>(ctx), Make<Deprecate>(ctx), Make<Deprecate>(ctx));
        Assert.AreEqual(1, power.GetLines(AutomatonKeyword.Compile).Count(), "Full Release adds one Compile line.");

        var none = await Compile(ctx, Make<Deprecate>(ctx), Make<Deprecate>(ctx), Make<Fragment>(ctx));
        Assert.AreEqual(0, none.GetLines(AutomatonKeyword.Compile).Count(), "Plain value effects add no Compile line.");
    }

    // The Encode / Compile keyword and the registered effects never drift apart: every Automaton card has
    // the Encode keyword exactly when it registered Encode effects (starter Strike and Defend register
    // them but get the keyword only when something encodes them), and Compile likewise.
    [CardTest(typeof(Automaton.AutomatonCode.Core.Automaton))]
    public Task KeywordsMatchRegisteredEffectsOnEveryCard(TestContext ctx)
    {
        foreach (var card in ModelDb.AllCards.OfType<AutomatonCardModel>())
        {
            var latent = card is StrikeAutomaton or DefendAutomaton;
            Assert.AreEqual(card.Encodings.Any() && !latent, card.Keywords.Contains(AutomatonKeyword.Encode),
                $"{card.GetType().Name}: Encode keyword must match its registered Encode effects.");
            Assert.AreEqual(card.Compilations.Any(), card.Keywords.Contains(AutomatonKeyword.Compile),
                $"{card.GetType().Name}: Compile keyword must match its registered Compile effects.");
        }

        return Task.CompletedTask;
    }

    // Strike and Defend are not Encode cards. Whatever encodes them (the starter relic placing them, or
    // Platinum Core when they are played) makes them one by putting them into the Encode pile; the shared
    // canonical card is never touched, so nothing leaks into the next combat or test run.
    [CardTest(typeof(Automaton.AutomatonCode.Core.Automaton))]
    public async Task StrikeAndDefendBecomeEncodeCardsOnlyWhenEncoded(TestContext ctx)
    {
        await ctx.PlayCard(await ctx.AddCardToHand<Boost>()); // flush BronzeCore's opening batch
        Assert.IsTrue(!ModelDb.Card<StrikeAutomaton>().Keywords.Contains(AutomatonKeyword.Encode),
            "The canonical Strike must never gain the keyword.");

        var plain = await ctx.AddCardToHand<StrikeAutomaton>();
        await ctx.PlayCard(plain, ctx.Combat.HittableEnemies.First());
        Assert.IsTrue(!plain.Keywords.Contains(AutomatonKeyword.Encode) && !ctx.Player.EncodePile.Contains(plain),
            "Without Platinum Core a played Strike is just a Strike: no keyword, not encoded.");

        await RelicCmd.Obtain<PlatinumCore>(ctx.Player);
        var forced = await ctx.AddCardToHand<StrikeAutomaton>();
        await ctx.PlayCard(forced, ctx.Combat.HittableEnemies.First());
        Assert.IsTrue(ctx.Player.EncodePile.Contains(forced), "Platinum Core encodes the played Strike.");
        Assert.IsTrue(forced.Keywords.Contains(AutomatonKeyword.Encode),
            "A Strike in the Encode pile has the Encode keyword.");

        var placed = Make<DefendAutomaton>(ctx);
        await AutomatonCmd.EncodeCard(placed, new BlockingPlayerChoiceContext());
        Assert.IsTrue(placed.Keywords.Contains(AutomatonKeyword.Encode),
            "A Defend placed directly into the Encode pile (starter relic path) has the Encode keyword.");
        Assert.IsTrue(!ModelDb.Card<DefendAutomaton>().Keywords.Contains(AutomatonKeyword.Encode),
            "The canonical Defend must never gain the keyword.");
    }
}
