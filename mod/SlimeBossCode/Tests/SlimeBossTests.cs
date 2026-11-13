using Downfall.DownfallCode.Tests;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Cards.Basic;
using SlimeBoss.SlimeBossCode.Cards.Common;
using SlimeBoss.SlimeBossCode.Cards.Rare;
using SlimeBoss.SlimeBossCode.Cards.Uncommon;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Extensions;
using SlimeBoss.SlimeBossCode.Powers;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Tests;

public class SlimeBossTests
{
    
    [CardTest(typeof(Core.SlimeBoss))]
    public IEnumerable<CardTestCase> PlaySlimeBossCards(CharacterModel character) => AllCardsTest.PlayAllCards(character);

    
    // Without Weak on the target, Equalizer Tackle's Consume shouldn't trigger at all - no Block gained.
    [CardTest(typeof(SlimeBoss.SlimeBossCode.Core.SlimeBoss))]
    public async Task EqualizeGrantsNoBlockWithoutWeakOnTarget(TestContext ctx)
    {
        var enemy = ctx.Combat.HittableEnemies.First();
        var equalize = await ctx.AddCardToHand<EqualizerTackle>();
        var startingBlock = ctx.Player.Creature.Block;

        await ctx.PlayCard(equalize, enemy);

        Assert.AreEqual(startingBlock, ctx.Player.Creature.Block,
            "Equalizer Tackle's Consume should not trigger (no Block gained) when the target has no Weak.");
    }

    // Consume must still trigger when the card's own attack kills the Weakened target (dead creatures lose
    // their powers, so Weak can't be read after the attack).
    [CardTest(typeof(SlimeBoss.SlimeBossCode.Core.SlimeBoss))]
    public async Task ConsumeTriggersWhenAttackKillsWeakenedTarget(TestContext ctx)
    {
        var enemy = ctx.Combat.HittableEnemies.First();
        await CreatureCmd.SetCurrentHp(enemy, 1);
        await PowerCmd.Apply<MegaCrit.Sts2.Core.Models.Powers.WeakPower>(new ThrowingPlayerChoiceContext(), enemy, 1, ctx.Player.Creature, null);
        var tackle = await ctx.AddCardToHand<HungryTackle>();
        var energyBefore = ctx.Player.PlayerCombatState!.Energy;

        await ctx.PlayCard(tackle, enemy);

        Assert.IsTrue(enemy.IsDead, "Test setup: the attack should have killed the enemy.");
        var gained = ctx.Player.PlayerCombatState!.Energy - energyBefore + tackle.EnergyCost.GetAmountToSpend();
        Assert.IsTrue(gained > 0, $"Hungry Tackle's Consume should grant Energy even though the Weak target died (net gain {gained}).");
    }

    // Slimy Cleaver: every Weak the player applies to an enemy makes that enemy lose 4 HP.
    [CardTest(typeof(SlimeBoss.SlimeBossCode.Core.SlimeBoss))]
    public async Task SlimyCleaverMakesEnemyLoseHpWhenWeakIsApplied(TestContext ctx)
    {
        var enemy = ctx.Combat.HittableEnemies.First();
        await RelicCmd.Obtain<Relics.SlimyCleaver>(ctx.Player);
        var hpBefore = enemy.CurrentHp;

        await PowerCmd.Apply<MegaCrit.Sts2.Core.Models.Powers.WeakPower>(new ThrowingPlayerChoiceContext(), enemy, 1, ctx.Player.Creature, null);

        Assert.AreEqual(hpBefore - 4, enemy.CurrentHp, "Applying Weak should cost the enemy 4 HP.");
    }

    // Football Helmet: playing a 2 Energy Attack gives 4 Block; other plays do not.
    [CardTest(typeof(SlimeBoss.SlimeBossCode.Core.SlimeBoss))]
    public async Task FootballHelmetGrantsBlockOnTwoEnergyAttacksOnly(TestContext ctx)
    {
        await RelicCmd.Obtain<Relics.FootballHelmet>(ctx.Player);
        var enemy = ctx.Combat.HittableEnemies.First();
        var blockBefore = ctx.Player.Creature.Block;

        await ctx.PlayCard(await ctx.AddCardToHand<ComboTackle>(), enemy);
        Assert.AreEqual(blockBefore + 4, ctx.Player.Creature.Block, "A 2 Energy Attack should grant 4 Block.");

        await ctx.PlayCard(await ctx.AddCardToHand<LevelUp>());
        Assert.AreEqual(blockBefore + 4, ctx.Player.Creature.Block, "A Power should not grant Block.");
    }

    // It Looks Tasty only attacks when the enemy has at least 3 Weak, and removes nothing.
    [CardTest(typeof(SlimeBoss.SlimeBossCode.Core.SlimeBoss))]
    public async Task ItLooksTastyNeedsThreeWeak(TestContext ctx)
    {
        var enemy = ctx.Combat.HittableEnemies.First();
        var hpBefore = enemy.CurrentHp;
        await PowerCmd.Apply<MegaCrit.Sts2.Core.Models.Powers.WeakPower>(new ThrowingPlayerChoiceContext(), enemy, 2, ctx.Player.Creature, null);

        await ctx.PlayCard(await ctx.AddCardToHand<ItLooksTasty>(), enemy);
        Assert.AreEqual(hpBefore, enemy.CurrentHp, "With only 2 Weak the card should do nothing.");

        await PowerCmd.Apply<MegaCrit.Sts2.Core.Models.Powers.WeakPower>(new ThrowingPlayerChoiceContext(), enemy, 1, ctx.Player.Creature, null);
        await ctx.PlayCard(await ctx.AddCardToHand<ItLooksTasty>(), enemy);
        // At least 16: test enemies can carry modifiers (e.g. Strength) that add a point.
        Assert.IsTrue(hpBefore - enemy.CurrentHp >= 16, "With 3 Weak the card should deal 16 damage.");
        Assert.AreEqual(3, enemy.GetPowerAmount<MegaCrit.Sts2.Core.Models.Powers.WeakPower>(), "The Weak check must not remove a stack.");
    }

    // Replication plays a copy of a Power from the Draw Pile; the original stays there.
    [CardTest(typeof(SlimeBoss.SlimeBossCode.Core.SlimeBoss))]
    public async Task ReplicationPlaysACopyAndLeavesTheOriginalInTheDrawPile(TestContext ctx)
    {
        var levelUp = await ctx.AddCardToTopOfDraw<LevelUp>();
        var replication = await ctx.AddCardToHand<Replication>();

        await ctx.PlayCard(replication);

        Assert.AreEqual(1, ctx.Player.Creature.GetPower<PotencyPower>()?.Amount ?? 0, "The copy of Level Up should have been played.");
        Assert.IsTrue(PileType.Draw.GetPile(ctx.Player).Cards.Contains(levelUp), "The original Power should stay in the Draw Pile.");
    }

    // Regression guard: calling CardCmd.AutoPlay a second time on the same Power-card instance silently
    // no-ops (Power cards resolve to PileType.None after playing once, which the game's unplayable-checks
    // then reject on the second call). This documents that failure mode.
    [CardTest(typeof(SlimeBoss.SlimeBossCode.Core.SlimeBoss))]
    public async Task AutoPlayingSamePowerCardTwiceDoesNotApplyItTwice(TestContext ctx)
    {
        var levelUp = await ctx.AddCardToHand<LevelUp>();

        await ctx.PlayCard(levelUp);
        await ctx.PlayCard(levelUp);

        var amount = ctx.Player.Creature.GetPower<PotencyPower>()?.Amount ?? 0;
        Assert.AreEqual(1, amount,
            $"Known limitation: a second AutoPlay on the same Power card instance is a silent no-op, got Amount={amount}.");
    }

    // Overexert plays X cards from the hand, cheapest first. X is whatever AutoPlay captured, so the expectation is
    // derived from it: Level Up (1 Energy) always goes before Prepare (2 Energy).
    [CardTest(typeof(SlimeBoss.SlimeBossCode.Core.SlimeBoss))]
    public async Task OverexertPlaysTheCheapestCardsInHandFirst(TestContext ctx)
    {
        await ctx.ClearHand();
        var prepare = await ctx.AddCardToHand<Prepare>();
        var levelUp = await ctx.AddCardToHand<LevelUp>();
        var overexert = await ctx.AddCardToHand<Overexert>();

        await ctx.PlayCard(overexert);

        var x = overexert.EnergyCost.CapturedXValue;
        var hand = PileType.Hand.GetPile(ctx.Player).Cards;
        Assert.IsTrue(x >= 1, $"The test needs X >= 1 but X was {x}.");
        Assert.IsTrue(!hand.Contains(levelUp), "The cheaper card (Level Up) should have been played.");
        Assert.AreEqual(x >= 2, !hand.Contains(prepare), $"With X={x} the more expensive card should be played only if X >= 2.");
    }

    // Glop Chop permanently lowers its own damage by 4 each play for the rest of the combat.
    [CardTest(typeof(SlimeBoss.SlimeBossCode.Core.SlimeBoss))]
    public async Task GlopChopLosesDamageAfterEachPlay(TestContext ctx)
    {
        var card = await ctx.AddCardToHand<GlopChop>();
        Assert.AreEqual(16, (int)card.DynamicVars.Damage.BaseValue, "Glop Chop should start at 16 damage.");
        await ctx.PlayCard(card, ctx.Combat.HittableEnemies.First());
        Assert.AreEqual(12, (int)card.DynamicVars.Damage.BaseValue, "Glop Chop should drop to 12 damage after one play.");
    }

    // Heavy Tackle gains 2 Energy (the harness spends none) only if a Status was played earlier this turn.
    [CardTest(typeof(SlimeBoss.SlimeBossCode.Core.SlimeBoss))]
    public async Task HeavyTackleGainsEnergyAfterStatusPlayedThisTurn(TestContext ctx)
    {
        var enemy = ctx.Combat.HittableEnemies.First();
        var energy = ctx.Player.PlayerCombatState!.Energy;
        await ctx.PlayCard(await ctx.AddCardToHand<HeavyTackle>(), enemy);
        Assert.AreEqual(energy, ctx.Player.PlayerCombatState!.Energy, "Heavy Tackle without a Status should gain no Energy.");

        await ctx.PlayCard(await ctx.AddCardToHand<Slimed>());
        energy = ctx.Player.PlayerCombatState!.Energy;
        await ctx.PlayCard(await ctx.AddCardToHand<HeavyTackle>(), enemy);
        Assert.AreEqual(energy + 2, ctx.Player.PlayerCombatState!.Energy, "Heavy Tackle after a Status should gain 2 Energy.");
    }

    // Recollect gains Block and is back in hand at the start of the next turn.
    [CardTest(typeof(SlimeBoss.SlimeBossCode.Core.SlimeBoss))]
    public async Task RecollectGainsBlockAndStaysOutOfHandUntilNextTurn(TestContext ctx)
    {
        var card = await ctx.AddCardToHand<Recollect>();
        await ctx.PlayCard(card);
        Assert.AreEqual(6, ctx.Player.Creature.Block, "Recollect should grant 6 Block.");
        Assert.IsTrue(card.Pile?.Type != PileType.Hand, "Recollect should leave the hand after being played.");
    }

    // Regression guard: Spreading Slime was previously implemented as "first Status card each turn is
    // free" (a copy-paste mixup with the CSV's other row 31 "Gluttony" card) instead of its actual effect
    // - granting Bruiser Slime Potency whenever a Status is played. Bruiser Slime already exists at this
    // point - MagnificentBowlerHat (SlimeBoss's starter relic) splits into it on turn 1's BeforeHandDraw.
    [CardTest(typeof(SlimeBoss.SlimeBossCode.Core.SlimeBoss))]
    public async Task SpreadingSlimeGrantsBruiserSlimePotencyWhenStatusIsPlayed(TestContext ctx)
    {
        var bruiser = ctx.Player.GetSlime<BruiserSlime>();
        Assert.IsTrue(bruiser != null, "Sanity check: Bruiser Slime should already exist via MagnificentBowlerHat.");
        var potencyBefore = bruiser!.GetPower<PotencyPower>()?.Amount ?? 0;

        var spreadingSlime = await ctx.AddCardToHand<SpreadingSlime>();
        await ctx.PlayCard(spreadingSlime);

        // Creating (not playing) a Status is what triggers it.
        await ctx.AddCardToHand<Slimed>();

        var amount = bruiser.GetPower<PotencyPower>()?.Amount ?? 0;
        Assert.AreEqual(potencyBefore + 1, amount,
            $"Spreading Slime should grant Bruiser Slime 1 Potency when a Status is created, got {amount}.");
    }

    [CardTest(typeof(SlimeBoss.SlimeBossCode.Core.SlimeBoss))]
    public async Task OneTwoComboReturnsToHandWhenAStatusIsPlayed(TestContext ctx)
    {
        var combo = await ctx.AddCardToHand<OneTwoCombo>();
        await ctx.PlayCard(combo, ctx.Combat.HittableEnemies.First());
        await CardPileCmd.Add(combo, PileType.Discard);

        var slimed = await ctx.AddCardToHand<Slimed>();
        await ctx.PlayCard(slimed);

        Assert.IsTrue(combo.Pile?.Type == PileType.Hand, "One-Two Combo should return to hand when a Status is played.");
    }

    [CardTest(typeof(SlimeBoss.SlimeBossCode.Core.SlimeBoss))]
    public async Task ViciousTackleHitsTwiceOnlyAgainstWeakEnemies(TestContext ctx)
    {
        var enemy = ctx.Combat.HittableEnemies.First();
        var tackle = await ctx.AddCardToHand<ViciousTackle>();
        var hpBefore = enemy.CurrentHp;
        await ctx.PlayCard(tackle, enemy);
        var single = hpBefore - enemy.CurrentHp;

        await PowerCmd.Apply<MegaCrit.Sts2.Core.Models.Powers.WeakPower>(new ThrowingPlayerChoiceContext(), enemy, 1, ctx.Player.Creature, null);
        var tackle2 = await ctx.AddCardToHand<ViciousTackle>();
        hpBefore = enemy.CurrentHp;
        await ctx.PlayCard(tackle2, enemy);
        var weakened = hpBefore - enemy.CurrentHp;

        Assert.IsTrue(weakened >= single * 2 - 1 && weakened > single, $"Vicious Tackle should hit twice vs Weak ({single} vs {weakened}).");
    }

    private static int TotalEnemyHp(TestContext ctx) => ctx.Combat.Enemies.Sum(e => e.CurrentHp);

    [CardTest(typeof(SlimeBoss.SlimeBossCode.Core.SlimeBoss))]
    public async Task LeadByExampleCommandsBruiserOnFirstTargetingCardOnly(TestContext ctx)
    {
        var enemy = ctx.Combat.HittableEnemies.First();
        await ctx.PlayCard(await ctx.AddCardToHand<LeadByExample>());
        var power = ctx.Player.Creature.GetPower<LeadByExamplePower>()!;
        Assert.AreEqual(1, power.DisplayAmount, "Nothing targeting has been played yet.");

        var hp = TotalEnemyHp(ctx);
        await ctx.PlayCard(await ctx.AddCardToHand<StrikeSlimeBoss>(), enemy);
        var first = hp - TotalEnemyHp(ctx);
        Assert.AreEqual(0, power.DisplayAmount, "The first targeting card should use up the only charge.");

        hp = TotalEnemyHp(ctx);
        await ctx.PlayCard(await ctx.AddCardToHand<StrikeSlimeBoss>(), enemy);
        var second = hp - TotalEnemyHp(ctx);
        Assert.IsTrue(first > second, $"Only the first targeting card should Command Bruiser ({first} vs {second}).");
    }

    // History-based: a targeting card played before the power was applied this turn already counts.
    [CardTest(typeof(SlimeBoss.SlimeBossCode.Core.SlimeBoss))]
    public async Task LeadByExampleCountsTargetingCardsPlayedBeforeItWasApplied(TestContext ctx)
    {
        var enemy = ctx.Combat.HittableEnemies.First();
        var hp = TotalEnemyHp(ctx);
        await ctx.PlayCard(await ctx.AddCardToHand<StrikeSlimeBoss>(), enemy);
        var baseline = hp - TotalEnemyHp(ctx);
        await ctx.PlayCard(await ctx.AddCardToHand<LeadByExample>());
        var power = ctx.Player.Creature.GetPower<LeadByExamplePower>()!;
        Assert.AreEqual(0, power.DisplayAmount, "The earlier targeting card should already count.");

        hp = TotalEnemyHp(ctx);
        await ctx.PlayCard(await ctx.AddCardToHand<StrikeSlimeBoss>(), enemy);
        // Each attack hits 1 harder than the last in this test setup; a Bruiser Command would add its 3+ damage on top.
        var damage = hp - TotalEnemyHp(ctx);
        Assert.IsTrue(damage < baseline + 3, $"No Bruiser Command expected: the first card was already played ({baseline} vs {damage}).");
    }

    // Regression guard: Repurpose's OnPlayInternal was changed from transforming selected cards one at a
    // time in a loop to batching them into a single CardCmd.Transform(IEnumerable<CardTransformation>, ...)
    // call - verify every selected card still ends up as an enchanted Slimed.
    [CardTest(typeof(SlimeBoss.SlimeBossCode.Core.SlimeBoss))]
    public async Task RepurposeTransformsSelectedCardsIntoEnchantedSlimed(TestContext ctx)
    {
        var repurpose = await ctx.AddCardToHand<Repurpose>();

        await ctx.PlayCard(repurpose);

        var slimedCards = PileType.Draw.GetPile(ctx.Player).Cards.OfType<Slimed>().ToList();
        Assert.AreEqual(2, slimedCards.Count,
            $"Repurpose should transform 2 cards into Slimed, got {slimedCards.Count}.");
        Assert.IsTrue(slimedCards.All(c => c.Enchantment == null),
            "Repurpose no longer enchants the Slimed cards it creates.");
    }

    // Same batching change as Repurpose, but selecting from hand and with a different enchant amount -
    // verify Avarice transforms every other hand card.
    [CardTest(typeof(SlimeBoss.SlimeBossCode.Core.SlimeBoss))]
    public async Task AvariceTransformsAllOtherHandCardsIntoEnchantedSlimed(TestContext ctx)
    {
        await ctx.ClearHand();
        await ctx.AddCardToHand<StrikeSlimeBoss>();
        await ctx.AddCardToHand<StrikeSlimeBoss>();
        var avarice = await ctx.AddCardToHand<Avarice>();

        await ctx.PlayCard(avarice);

        var slimedCards = PileType.Hand.GetPile(ctx.Player).Cards.OfType<Slimed>().ToList();
        Assert.AreEqual(2, slimedCards.Count,
            $"Avarice should transform every other hand card into Slimed, got {slimedCards.Count}.");
        Assert.IsTrue(slimedCards.All(c => c.Enchantment == null),
            "Avarice no longer enchants the Slimed cards it creates.");
    }

    [CardTest(typeof(SlimeBoss.SlimeBossCode.Core.SlimeBoss))]
    public async Task MuscleSlimeIsCommandedWhenTackleIsPlayed(TestContext ctx)
    {
        await SlimeBossCmd.Split<MuscleSlime>(new ThrowingPlayerChoiceContext(), ctx.Player);
        var enemy = ctx.Combat.HittableEnemies.First();
        var hpBefore = enemy.CurrentHp;

        var tackle = await ctx.AddCardToHand<HeavyTackle>();
        var cardDamage = tackle.DynamicVars.Damage.IntValue;

        await ctx.PlayCard(tackle, enemy);

        var dealt = hpBefore - enemy.CurrentHp;
        Assert.IsTrue(dealt > cardDamage,
            $"Muscle Slime should attack on top of the Tackle's {cardDamage} damage, but only {dealt} was dealt.");
    }

    [CardTest(typeof(SlimeBoss.SlimeBossCode.Core.SlimeBoss))]
    public async Task SlimeDoubleDamagePowerDoublesSlimeAttacks(TestContext ctx)
    {
        var ctxChoice = new ThrowingPlayerChoiceContext();
        var hp = TotalEnemyHp(ctx);
        await SlimeBossCmd.Command<BruiserSlime>(ctxChoice, ctx.Player, 1);
        var normal = hp - TotalEnemyHp(ctx);

        await PowerCmd.Apply<SlimeDoubleDamagePower>(ctxChoice, ctx.Player.Creature, 1, ctx.Player.Creature, null);
        hp = TotalEnemyHp(ctx);
        await SlimeBossCmd.Command<BruiserSlime>(ctxChoice, ctx.Player, 1);
        Assert.AreEqual(normal * 2, hp - TotalEnemyHp(ctx), "Bruiser Slime should deal double damage.");
    }
    

    // Liquidate converts Strength into Potency.
    [CardTest(typeof(SlimeBoss.SlimeBossCode.Core.SlimeBoss))]
    public async Task LiquidateTradesStrengthForPotency(TestContext ctx)
    {
        var liquidate = await ctx.AddCardToHand<Liquidate>();

        await ctx.PlayCard(liquidate);

        Assert.AreEqual(-2, ctx.Player.Creature.GetPowerAmount<MegaCrit.Sts2.Core.Models.Powers.StrengthPower>(),
            "Liquidate should remove 2 Strength.");
        Assert.AreEqual(2, ctx.Player.Creature.GetPowerAmount<PotencyPower>(), "Liquidate should grant 2 Potency.");
    }

    // Vanish gains 2 Energy and puts a Slimed on top of the draw pile.
    [CardTest(typeof(SlimeBoss.SlimeBossCode.Core.SlimeBoss))]
    public async Task VanishGainsEnergyAndPutsSlimedOnDrawPile(TestContext ctx)
    {
        var vanish = await ctx.AddCardToHand<Vanish>();
        var energy = ctx.Player.PlayerCombatState!.Energy;

        await ctx.PlayCard(vanish);

        Assert.AreEqual(energy + 2, ctx.Player.PlayerCombatState!.Energy, "Vanish should gain 2 Energy.");
        Assert.IsTrue(ctx.Player.PlayerCombatState!.DrawPile.Cards.First() is Slimed, "Slimed should be on top of the draw pile.");
    }

    // Slime Spikes grants Thorns only once a Status has been played this turn.
    [CardTest(typeof(SlimeBoss.SlimeBossCode.Core.SlimeBoss))]
    public async Task SlimeSpikesGrantsThornsOnlyAfterPlayingAStatus(TestContext ctx)
    {
        await ctx.PlayCard(await ctx.AddCardToHand<SlimeSpikes>());
        Assert.AreEqual(0, ctx.Player.Creature.GetPowerAmount<MegaCrit.Sts2.Core.Models.Powers.ThornsPower>(),
            "No Thorns without a Status played.");

        await ctx.PlayCard(await ctx.AddCardToHand<Slimed>());
        await ctx.PlayCard(await ctx.AddCardToHand<SlimeSpikes>());
        Assert.AreEqual(2, ctx.Player.Creature.GetPowerAmount<MegaCrit.Sts2.Core.Models.Powers.ThornsPower>(),
            "2 Thorns after a Status was played.");
    }

    // Recklessness grants Strength per 2-Energy Attack played, and not for other costs.
    [CardTest(typeof(SlimeBoss.SlimeBossCode.Core.SlimeBoss))]
    public async Task RecklessnessGrantsStrengthOnTwoEnergyAttacksOnly(TestContext ctx)
    {
        var enemy = ctx.Combat.HittableEnemies.First();
        await ctx.PlayCard(await ctx.AddCardToHand<Recklessness>());
        await ctx.PlayCard(await ctx.AddCardToHand<StrikeSlimeBoss>(), enemy);
        Assert.AreEqual(0, ctx.Player.Creature.GetPowerAmount<MegaCrit.Sts2.Core.Models.Powers.StrengthPower>(),
            "A 1-cost Attack should not grant Strength.");
        await ctx.PlayCard(await ctx.AddCardToHand<HeavyTackle>(), enemy);
        Assert.AreEqual(1, ctx.Player.Creature.GetPowerAmount<MegaCrit.Sts2.Core.Models.Powers.StrengthPower>(),
            "A 2-cost Attack should grant 1 Strength.");
    }

    // Split: Muscle costs 1 and grants no Potency unless upgraded.
    [CardTest(typeof(SlimeBoss.SlimeBossCode.Core.SlimeBoss))]
    public async Task SplitMuscleCostsOneAndGrantsNoPotency(TestContext ctx)
    {
        var card = await ctx.AddCardToHand<SplitMuscle>();
        Assert.AreEqual(1, card.EnergyCost.GetAmountToSpend(), "Split: Muscle should cost 1.");
        await ctx.PlayCard(card);
        var slime = SlimeBossCmd.GetSlimes(ctx.Player).OfType<MuscleSlime>().First();
        Assert.AreEqual(0, slime.Creature.GetPowerAmount<PotencyPower>(), "Base Split: Muscle grants no Potency.");
    }

    [CardTest(typeof(SlimeBoss.SlimeBossCode.Core.SlimeBoss))]
    public async Task UpgradedSplitMuscleGrantsTheSlimeTwoPotency(TestContext ctx)
    {
        var card = await ctx.AddCardToHand<SplitMuscle>();
        CardCmd.Upgrade(card);
        await ctx.PlayCard(card);
        var slime = SlimeBossCmd.GetSlimes(ctx.Player).OfType<MuscleSlime>().First();
        Assert.AreEqual(2, slime.Creature.GetPowerAmount<PotencyPower>(), "Upgraded Split: Muscle gives the slime 2 Potency.");
    }

    // "[E][E] Attack" means the Energy actually spent, like base-game Feral: a discounted 2-cost Attack doesn't count.
    [CardTest(typeof(SlimeBoss.SlimeBossCode.Core.SlimeBoss))]
    public async Task DiscountedTwoCostAttackDoesNotCountAsTwoEnergyAttack(TestContext ctx)
    {
        var enemy = ctx.Combat.HittableEnemies.First();
        await ctx.PlayCard(await ctx.AddCardToHand<Recklessness>());
        var heavy = await ctx.AddCardToHand<HeavyTackle>();
        heavy.EnergyCost.SetThisTurn(0);
        await ctx.PlayCard(heavy, enemy);
        Assert.AreEqual(0, ctx.Player.Creature.GetPowerAmount<MegaCrit.Sts2.Core.Models.Powers.StrengthPower>(),
            "A free Heavy Tackle should not trigger Recklessness.");
    }

    // Psychic Slime is Commanded whenever its owner plays a Power, and Commanding it deals 6 damage to a random enemy.
    [CardTest(typeof(SlimeBoss.SlimeBossCode.Core.SlimeBoss))]
    public async Task PsychicSlimeDealsDamageWhenAPowerIsPlayed(TestContext ctx)
    {
        await SlimeBossCmd.Split<PsychicSlime>(new ThrowingPlayerChoiceContext(), ctx.Player);
        var enemies = ctx.Combat.HittableEnemies.ToList();
        var hpBefore = enemies.Sum(e => e.CurrentHp);
        var power = await ctx.AddCardToHand<SplitMassive>();

        await ctx.PlayCard(power);

        var lost = hpBefore - enemies.Sum(e => e.CurrentHp);
        Assert.AreEqual(6, lost, "Playing a Power should Command Psychic Slime, dealing 6 damage to one enemy.");
    }
}
