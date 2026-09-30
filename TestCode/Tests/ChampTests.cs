using Champ.ChampCode.Cards.Basic;
using Champ.ChampCode.Cards.Uncommon;
using Champ.ChampCode.Enchantments;
using Champ.ChampCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.TestCode;

public class ChampTests
{
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
    [CardTest(typeof(Hermit.HermitCode.Core.Hermit))]
    public async Task StrikeOfGeniusFallsBackToBasicStrikeWhenNoOtherStrikeExists(TestContext ctx)
    {
        var handBefore = ctx.Player.Hand.ToList();

        var power = await PowerCmd.Apply<StrikeOfGeniusPower>(new BlockingPlayerChoiceContext(),
            ctx.Player.Creature, 3, ctx.Player.Creature, null);
        Assert.IsTrue(power != null, "Sanity check: StrikeOfGeniusPower should have been applied.");

        await power!.BeforeHandDraw(ctx.Player, new BlockingPlayerChoiceContext(), ctx.Combat);

        var generated = ctx.Player.Hand.Except(handBefore).ToList();
        Assert.AreEqual(3, generated.Count,
            "Strike of Genius should still generate its full Amount when the only Strike card " +
            "the character has is Basic Strike.");
        Assert.IsTrue(generated.All(c => c.Rarity == CardRarity.Basic && c.Tags.Contains(CardTag.Strike)),
            "The fallback cards should be Basic Strike.");
    }

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
}
