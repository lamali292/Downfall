using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models;
using Champ.ChampCode.Cards.Basic;
using Champ.ChampCode.Cards.Uncommon;
using Champ.ChampCode.Enchantments;
using Champ.ChampCode.Extensions;
using Champ.ChampCode.Powers;
using Downfall.TestCode;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Champ.ChampCode.Teets;

public class ChampTests
{
    
    [CardTest(typeof(Champ.ChampCode.Core.Champ))]
    public IEnumerable<CardTestCase> PlayChampCards(CharacterModel character) => AllCardsTest.PlayAllCards(character);
    
    // Crowned makes a card free via its base cost, which is meaningless for X cards, so neither
    // X-energy nor X-star cards may be crowned.
    [CardTest(typeof(Champ.ChampCode.Core.Champ))]
    public async Task CrownedOnlyAllowsNumericCostCards(TestContext ctx)
    {
        var crowned = ModelDb.Enchantment<Crowned>();
        var strike = await ctx.AddCardToHand<StrikeIronclad>();
        var whirlwind = await ctx.AddCardToHand<Whirlwind>();
        var stardust = await ctx.AddCardToHand<Stardust>();

        Assert.IsTrue(crowned.CanEnchant(strike), "A numeric-cost card can be crowned.");
        Assert.IsTrue(!crowned.CanEnchant(whirlwind), "An X-energy card cannot be crowned.");
        Assert.IsTrue(!crowned.CanEnchant(stardust), "An X-star card cannot be crowned.");
    }

    // Regression guard: Vigor's damage bonus was only applying to Challenge's first hit, not the
    // second one triggered when the target has Strength, even though the card's description implies
    // both hits should be identical. Root cause was that the repeat used a second, separate
    // AttackCommand - Vigor is consumed after one AttackCommand.Execute(), so only the first hit got
    // it. Fixed by dealing both hits from a single AttackCommand (hitCount: 2) instead.
    [CardTest(typeof(Champ.ChampCode.Core.Champ))]
    public async Task VigorAppliesToBothChallengeHitsWhenTargetHasStrength(TestContext ctx)
    {
        var enemy = ctx.Combat.HittableEnemies.First();
        var startHp = enemy.CurrentHp;

        // Champion's Crown (Champ's starting relic) auto-enters a random stance on turn 1's draw and
        // immediately fires that stance's SkillBonus, which grants Vigor(2) on a Berserker Stance
        // coinflip. Clear that out so this test's expected damage isn't seed-dependent.
        await Champ.ChampCode.Core.ChampCmd.ClearStance(new BlockingPlayerChoiceContext(), ctx.Player);
        await PowerCmd.Remove<VigorPower>(ctx.Player.Creature);

        await PowerCmd.Apply<StrengthPower>(new BlockingPlayerChoiceContext(), enemy, 1, null, null);
        await PowerCmd.Apply<VigorPower>(new BlockingPlayerChoiceContext(), ctx.Player.Creature, 5,
            ctx.Player.Creature, null);

        var challenge = await ctx.AddCardToHand<Challenge>();
        await ctx.PlayCard(challenge, enemy);

        var totalDamage = startHp - enemy.CurrentHp;
        Assert.AreEqual(24, totalDamage,
            "Vigor should boost both Challenge hits when the target has Strength " +
            "((7 base + 5 vigor) * 2 hits = 24).");
    }

    // Regression guard: player-reported that Strike of Genius generates nothing for a Hermit who
    // has no Strike-tagged Attack cards other than Basic Strike. Root cause was that
    // CardFactory.GetDistinctForCombat (used to pick the random Strike cards) unconditionally
    // filters out Basic-rarity cards, so a pool whose only Strike Attack is Basic Strike resolves
    // to empty. Fixed by falling back to Basic Strike to fill any remaining slots.
    //
    // This test needs a non-Champ character with no Strike Attack besides Basic Strike (Hermit),
    // so it lives in HermitCode/Tests/HermitTests.cs instead of here - Hermit is its own
    // standalone-submod assembly (Hermit.csproj) now, and TestCode (compiled into Downfall.dll)
    // can't reference back into it (same circular-reference restriction as DownfallCode -> Hermit;
    // see ADR 0003). The reverse direction works fine: Hermit.csproj -> Downfall.csproj already
    // includes ChampCode (still bundled), so the moved test can reference StrikeOfGeniusPower
    // directly.

    // Regression guard: a Finisher played without a stance via the Signature enchantment never
    // triggered Dancing Master because PlayFinisher bailed out when the stance had no Finisher.
    // Only the first Finisher each turn should trigger it.
    [CardTest(typeof(Champ.ChampCode.Core.Champ))]
    public async Task DancingMasterTriggersOnceOnSignatureFinisherWithoutStance(TestContext ctx)
    {
        var enemy = ctx.Combat.HittableEnemies.First();
        await Champ.ChampCode.Core.ChampCmd.ClearStance(new BlockingPlayerChoiceContext(), ctx.Player);
        await PowerCmd.Apply<DancingMasterPower>(new BlockingPlayerChoiceContext(), ctx.Player.Creature, 1,
            ctx.Player.Creature, null);

        var first = await ctx.AddCardToHand<Execute>();
        var second = await ctx.AddCardToHand<Execute>();
        CardCmd.Enchant<Signature>(first, 1);
        CardCmd.Enchant<Signature>(second, 1);

        var energyBefore = ctx.Player.PlayerCombatState!.Energy;
        await ctx.PlayCard(first, enemy);
        Assert.AreEqual(energyBefore + 1, ctx.Player.PlayerCombatState!.Energy,
            "Dancing Master should trigger on a stanceless Signature Finisher.");

        await ctx.PlayCard(second, enemy);
        Assert.AreEqual(energyBefore + 1, ctx.Player.PlayerCombatState!.Energy,
            "Dancing Master should only trigger for the first Finisher each turn.");
    }

    // Crowned makes the card free via its base cost, so it also shows as free outside combat (deck view),
    // and does not flag the cost as upgraded.
    [CardTest(typeof(Champ.ChampCode.Core.Champ))]
    public async Task CrownedMakesCardFreeAndNotUpgraded(TestContext ctx)
    {
        var card = await ctx.AddCardToHand<Challenge>();
        Assert.IsTrue(card.EnergyCost.Canonical > 0, "Sanity check: Challenge should cost energy.");

        CardCmd.Enchant<Crowned>(card, 1);

        Assert.AreEqual(0, card.EnergyCost.GetWithModifiers(CostModifiers.None), "Crowned card should be free.");
        Assert.IsTrue(!card.EnergyCost.WasJustUpgraded, "Crowned should not flag the cost as upgraded.");
    }

    // Cards borrowed from other pools can have a star cost; Crowned should zero it too.
    [CardTest(typeof(Champ.ChampCode.Core.Champ))]
    public async Task CrownedAlsoZeroesStarCost(TestContext ctx)
    {
        var card = await ctx.AddCardToHand<MegaCrit.Sts2.Core.Models.Cards.AstralPulse>();
        Assert.IsTrue(card.BaseStarCost > 0, "Sanity check: Astral Pulse should have a star cost.");

        CardCmd.Enchant<Crowned>(card, 1);

        Assert.AreEqual(0, card.BaseStarCost, "Crowned should make the star cost free.");
        Assert.AreEqual(0, card.EnergyCost.GetWithModifiers(CostModifiers.None), "Energy cost should be free too.");
    }

    private static int FinisherEntryCount() =>
        CombatManager.Instance.History.Entries.OfType<Champ.ChampCode.History.FinisherEntry>().Count();

    private static async Task ResetChampState(TestContext ctx)
    {
        await Champ.ChampCode.Core.ChampCmd.ClearStance(new BlockingPlayerChoiceContext(), ctx.Player);
        await PowerCmd.Remove<VigorPower>(ctx.Player.Creature);
        await PlayerCmd.SetEnergy(5, ctx.Player);
    }

    // The Finisher descriptor drives playability and glow through the same "would it act" rule as execution,
    // so the UI cannot claim a Finisher works when it would fizzle (and vice versa).
    [CardTest(typeof(Champ.ChampCode.Core.Champ))]
    public async Task FinisherPlayableAndGlowMatchWhetherItActs(TestContext ctx)
    {
        await ResetChampState(ctx);
        var card = await ctx.AddCardToHand<Execute>();

        Assert.IsTrue(!card.CanPlay(), "A Finisher should be unplayable without a stance.");
        Assert.IsTrue(!card.ShouldGlowRed, "A Finisher that would not act should not glow red.");
        Assert.IsTrue(!Champ.ChampCode.Core.ChampCmd.FinisherCanAct(card), "Sanity check: it would not act.");

        CardCmd.Enchant<Signature>(card, 1);
        Assert.IsTrue(card.CanPlay(), "A Signature Finisher should be playable without a stance.");
        Assert.IsTrue(card.ShouldGlowRed, "A Signature Finisher that acts without a stance should glow red.");

        var plain = await ctx.AddCardToHand<Execute>();
        await Champ.ChampCode.Core.ChampCmd.EnterBerserkerStance(new BlockingPlayerChoiceContext(), ctx.Player);
        Assert.IsTrue(plain.CanPlay(), "A Finisher should be playable in a stance with a Finisher.");
        Assert.IsTrue(plain.ShouldGlowRed, "A Finisher that acts in the current stance should glow red.");
    }

    // A default Finisher resolves once and clears the stance.
    [CardTest(typeof(Champ.ChampCode.Core.Champ))]
    public async Task DefaultFinisherResolvesOnceAndClearsStance(TestContext ctx)
    {
        await ResetChampState(ctx);
        await Champ.ChampCode.Core.ChampCmd.EnterBerserkerStance(new BlockingPlayerChoiceContext(), ctx.Player);
        var before = FinisherEntryCount();

        await ctx.PlayCard(await ctx.AddCardToHand<Execute>(), ctx.Combat.HittableEnemies.First());

        Assert.AreEqual(before + 1, FinisherEntryCount(), "A default Finisher should record one history entry.");
        Assert.IsTrue(ctx.Player.ChampStance is Champ.ChampCode.Stance.ChampNoStance,
            "A default Finisher should clear the stance, but it is " + ctx.Player.ChampStance.GetType().Name);
    }

    // Execution declares repeat 2 and keeps the stance.
    [CardTest(typeof(Champ.ChampCode.Core.Champ))]
    public async Task ExecutionFinisherRepeatsTwiceAndKeepsStance(TestContext ctx)
    {
        await ResetChampState(ctx);
        await Champ.ChampCode.Core.ChampCmd.EnterBerserkerStance(new BlockingPlayerChoiceContext(), ctx.Player);
        var before = FinisherEntryCount();

        await ctx.PlayCard(await ctx.AddCardToHand<Champ.ChampCode.Cards.Ancient.Execution>(),
            ctx.Combat.HittableEnemies.First());

        Assert.AreEqual(before + 2, FinisherEntryCount(), "Execution should resolve its Finisher twice.");
        Assert.IsTrue(ctx.Player.ChampStance is Champ.ChampCode.Stance.ChampBerserkerStance,
            "Execution should not clear the stance.");
    }

    // All Out repeats by its Repeat var and keeps the stance.
    [CardTest(typeof(Champ.ChampCode.Core.Champ))]
    public async Task AllOutFinisherRepeatsByVarAndKeepsStance(TestContext ctx)
    {
        await ResetChampState(ctx);
        await Champ.ChampCode.Core.ChampCmd.EnterBerserkerStance(new BlockingPlayerChoiceContext(), ctx.Player);
        var before = FinisherEntryCount();

        var card = await ctx.AddCardToHand<AllOut>();
        var expected = card.DynamicVars.Repeat.IntValue;
        await ctx.PlayCard(card);

        Assert.AreEqual(before + expected, FinisherEntryCount(), "All Out should resolve its Finisher Repeat times.");
        Assert.IsTrue(ctx.Player.ChampStance is Champ.ChampCode.Stance.ChampBerserkerStance,
            "All Out should not clear the stance.");
    }

    // Steel Edge repeats its Finisher X times (at least once) and clears the stance.
    [CardTest(typeof(Champ.ChampCode.Core.Champ))]
    public async Task SteelEdgeFinisherRepeatsXTimes(TestContext ctx)
    {
        await ResetChampState(ctx);
        await Champ.ChampCode.Core.ChampCmd.EnterBerserkerStance(new BlockingPlayerChoiceContext(), ctx.Player);
        var before = FinisherEntryCount();
        var x = Math.Max(1, ctx.Player.PlayerCombatState!.Energy);

        await ctx.PlayCard(await ctx.AddCardToHand<Champ.ChampCode.Cards.Rare.SteelEdge>(),
            ctx.Combat.HittableEnemies.First());

        Assert.AreEqual(before + x, FinisherEntryCount(), "Steel Edge should resolve its Finisher X times.");
        Assert.IsTrue(ctx.Player.ChampStance is Champ.ChampCode.Stance.ChampNoStance,
            "Steel Edge should clear the stance.");
    }
}
