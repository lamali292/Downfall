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

    // Slimy Cleaver: the first two Consumes leave Weak untouched (no remove-and-reapply), later ones remove a stack.
    [CardTest(typeof(SlimeBoss.SlimeBossCode.Core.SlimeBoss))]
    public async Task SlimyCleaverLetsFirstTwoConsumesKeepWeak(TestContext ctx)
    {
        var enemy = ctx.Combat.HittableEnemies.First();
        await CreatureCmd.SetCurrentHp(enemy, 9999);
        await PowerCmd.Apply<MegaCrit.Sts2.Core.Models.Powers.WeakPower>(new ThrowingPlayerChoiceContext(), enemy, 3, ctx.Player.Creature, null);
        await RelicCmd.Obtain<Relics.SlimyCleaver>(ctx.Player);

        var weakAfter = new List<int>();
        for (var i = 0; i < 3; i++)
        {
            await ctx.PlayCard(await ctx.AddCardToHand<HungryTackle>(), enemy);
            weakAfter.Add(enemy.GetPower<MegaCrit.Sts2.Core.Models.Powers.WeakPower>()?.Amount ?? 0);
        }

        Assert.IsTrue(weakAfter.SequenceEqual([3, 3, 2]),
            $"Weak after three Consumes should be 3,3,2 but was {string.Join(",", weakAfter)}.");
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

    // End-to-end: Overexert itself, drawing a Power card and choosing to replay it, should apply it twice.
    // The fix overrides ModifyCardPlayCount directly on Overexert (it's a live combat hook listener while
    // its own OnPlayInternal is resolving), bumping just the chosen card's play count by 1 - no separate
    // modifier/power needed, unlike calling CardCmd.AutoPlay twice from the outside (silently no-ops the
    // second time for Power cards).
    [CardTest(typeof(SlimeBoss.SlimeBossCode.Core.SlimeBoss))]
    public async Task OverexertPlaysChosenPowerCardTwice(TestContext ctx)
    {
        await ctx.AddCardToTopOfDraw<LevelUp>();
        var overexert = await ctx.AddCardToHand<Overexert>();
        overexert.EnergyCost.CapturedXValue = 1;

        await ctx.PlayCard(overexert);

        var amount = ctx.Player.Creature.GetPower<PotencyPower>()?.Amount ?? 0;
        Assert.AreEqual(2, amount, $"Overexert should draw and play LevelUp twice, got Potency={amount}.");
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

    // Heavy Tackle costs 1 less per Status played this turn, including Statuses played before it was drawn.
    [CardTest(typeof(SlimeBoss.SlimeBossCode.Core.SlimeBoss))]
    public async Task HeavyTackleCostsLessPerStatusPlayedThisTurn(TestContext ctx)
    {
        var heavy = await ctx.AddCardToHand<HeavyTackle>();
        Assert.AreEqual(2, heavy.EnergyCost.GetAmountToSpend(), "Heavy Tackle should cost 2 with no Status played.");
        var slimed = await ctx.AddCardToHand<Slimed>();
        await ctx.PlayCard(slimed);
        Assert.AreEqual(1, heavy.EnergyCost.GetAmountToSpend(), "Heavy Tackle should cost 1 after one Status.");
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
        Assert.IsTrue(slimedCards.All(c => c.Enchantment is Swift { Amount: 1 }),
            "Each transformed Slimed should be enchanted with Swift 1.");
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
        Assert.IsTrue(slimedCards.All(c => c.Enchantment is Adroit { Amount: 5 }),
            "Each transformed Slimed should be enchanted with Adroit 5 (Avarice's Adroit amount).");
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

    // Vanish repeats its Block once per Status in hand.
    [CardTest(typeof(SlimeBoss.SlimeBossCode.Core.SlimeBoss))]
    public async Task VanishRepeatsBlockPerStatusInHand(TestContext ctx)
    {
        await ctx.ClearHand();
        await ctx.AddCardToHand<Slimed>();
        await ctx.AddCardToHand<Slimed>();
        var vanish = await ctx.AddCardToHand<Vanish>();
        var before = ctx.Player.Creature.Block;

        await ctx.PlayCard(vanish);

        Assert.AreEqual(before + 15, ctx.Player.Creature.Block, "Vanish should gain 5 Block 3 times (2 Statuses).");
    }

    // Slime Spikes costs 0 once a Status has been played this turn.
    [CardTest(typeof(SlimeBoss.SlimeBossCode.Core.SlimeBoss))]
    public async Task SlimeSpikesIsFreeAfterPlayingAStatus(TestContext ctx)
    {
        var spikes = await ctx.AddCardToHand<SlimeSpikes>();
        Assert.AreEqual(1, spikes.EnergyCost.GetAmountToSpend(), "Slime Spikes should cost 1 with no Status played.");

        var slimed = await ctx.AddCardToHand<Slimed>();
        await ctx.PlayCard(slimed);

        Assert.AreEqual(0, spikes.EnergyCost.GetAmountToSpend(), "Slime Spikes should cost 0 after a Status play.");
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

    // Psychic Slime is Commanded whenever its owner plays a Power, and Commanding it applies Soulburn.
    [CardTest(typeof(SlimeBoss.SlimeBossCode.Core.SlimeBoss))]
    public async Task PsychicSlimeAppliesSoulburnWhenAPowerIsPlayed(TestContext ctx)
    {
        await SlimeBossCmd.Split<PsychicSlime>(new ThrowingPlayerChoiceContext(), ctx.Player);
        var enemy = ctx.Combat.HittableEnemies.First();
        var power = await ctx.AddCardToHand<SplitMassive>();

        await ctx.PlayCard(power);

        Assert.AreEqual(6, enemy.GetPowerAmount<Downfall.DownfallCode.Powers.SoulBurnPower>(),
            "Playing a Power should Command Psychic Slime, applying 6 Soulburn.");
    }
}
