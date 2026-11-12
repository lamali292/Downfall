using Downfall.DownfallCode.Compatibility;
using Downfall.DownfallCode.Tests;
using Downfall.DownfallCode.Powers;
using Hexaghost.HexaghostCode.Cards.Common;
using Hexaghost.HexaghostCode.Cards.Multiplayer;
using Hexaghost.HexaghostCode.Cards.Basic;
using Hexaghost.HexaghostCode.Cards.Rare;
using Hexaghost.HexaghostCode.Cards.Uncommon;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Relics;

namespace Hexaghost.HexaghostCode.Tests;

public class HexaghostTests
{
    
    [CardTest(typeof(Hexaghost.HexaghostCode.Core.Hexaghost))]
    public IEnumerable<CardTestCase> PlayHexaghostCards(CharacterModel character) => AllCardsTest.PlayAllCards(character);


    
    // Regression guard for the HexaghostCardPlayPhases removal: Retract/Advance used to be dispatched
    // centrally off the card's keyword after/before OnPlay; now each card calls HexaghostCmd.Retract/
    // Advance itself from OnPlayInternal, so this checks the wheel actually still moves.
    [CardTest(typeof(Hexaghost.HexaghostCode.Core.Hexaghost))]
    public async Task BacktrackSmackRetractsTheWheel(TestContext ctx)
    {
        var wheelLength = HexaghostCmd.GetWheel(ctx.Player).Length;
        var before = HexaghostCmd.GetCurrentIndex(ctx.Player);

        await ctx.PlayCard(await ctx.AddCardToHand<BacktrackSmack>(), ctx.Combat.HittableEnemies.First());

        Assert.AreEqual((before + wheelLength - 1) % wheelLength, HexaghostCmd.GetCurrentIndex(ctx.Player),
            "BacktrackSmack should retract the wheel by one.");
    }

    [CardTest(typeof(Hexaghost.HexaghostCode.Core.Hexaghost))]
    public async Task AdvancingGuardAdvancesTheWheel(TestContext ctx)
    {
        var wheelLength = HexaghostCmd.GetWheel(ctx.Player).Length;
        var before = HexaghostCmd.GetCurrentIndex(ctx.Player);

        await ctx.PlayCard(await ctx.AddCardToHand<AdvancingGuard>());

        Assert.AreEqual((before + 1) % wheelLength, HexaghostCmd.GetCurrentIndex(ctx.Player),
            "AdvancingGuard should advance the wheel by one.");
    }

    // The flame the card was played on reacts (only its effect comes after the play), even when the card advanced the wheel.
    [CardTest(typeof(Hexaghost.HexaghostCode.Core.Hexaghost))]
    public async Task AdvancingCardTriggersTheFlameItWasPlayedOn(TestContext ctx)
    {
        HexaghostModel.CurrentIndex[ctx.Player] = 1; // the Skill-triggered Crushing flame
        var wheel = HexaghostCmd.GetWheel(ctx.Player);
        wheel[1].SetIgniteProgress();
        wheel[2].SetIgniteProgress();
        var floatCard = await ctx.AddCardToHand<Float>(); // Skill + Advance

        Assert.IsTrue(wheel[1].IsPrimedBy(floatCard), "Float is played on flame 1, so it primes it.");
        await ctx.PlayCard(floatCard);

        Assert.AreEqual(2, HexaghostCmd.GetCurrentIndex(ctx.Player), "Float advanced the wheel.");
        Assert.IsTrue(wheel[1].IsIgnited, "The flame Float was played on ignites.");
        Assert.IsTrue(!wheel[2].IsIgnited, "The flame Float advanced to does not.");
    }

    // Any card igniting the flame by hand, when its own play also ignites it (primed), still gets the prime ignition
    // after the play: twice the damage of an unprimed Kindle.
    [CardTest(typeof(Hexaghost.HexaghostCode.Core.Hexaghost))]
    public async Task KindleIgnitesAgainWhenItsPlayPrimedTheFlame(TestContext ctx)
    {
        HexaghostModel.CurrentIndex[ctx.Player] = 1; // Skill-triggered Crushing flame
        var flame = HexaghostCmd.GetCurrentFlame(ctx.Player);
        int Hp() => ctx.Combat.Enemies.Sum(e => e.CurrentHp);

        var before = Hp();
        await ctx.PlayCard(await ctx.AddCardToHand<Kindle>());
        var single = before - Hp();
        Assert.IsTrue(single > 0, "Kindle's ignite deals damage.");

        flame.Extinguish();
        flame.SetIgniteProgress(); // the next Skill ignites it
        var primedKindle = await ctx.AddCardToHand<Kindle>();
        Assert.IsTrue(flame.IsPrimedBy(primedKindle), "Kindle primes the full flame.");

        before = Hp();
        await ctx.PlayCard(primedKindle);
        Assert.AreEqual(2 * single, before - Hp(), "The prime ignites the flame a second time.");
    }

    // Not special to Kindle: Haunting Echo ignites the primed flame by hand, then the prime ignites it again.
    [CardTest(typeof(Hexaghost.HexaghostCode.Core.Hexaghost))]
    public async Task HauntingEchoIgnitesPrimedFlameTwice(TestContext ctx)
    {
        var flame = HexaghostCmd.GetCurrentFlame(ctx.Player); // Searing: Soulburn when ignited
        flame.SetIgniteProgress();
        int Soulburn() => ctx.Combat.Enemies.Sum(e => e.GetPowerAmount<SoulBurnPower>());

        await ctx.PlayCard(await ctx.AddCardToHand<HauntingEcho>(), ctx.Combat.HittableEnemies.First());
        var twoIgnitions = Soulburn();

        flame.Extinguish();
        flame.SetIgniteProgress();
        var before = Soulburn();
        await ctx.PlayCard(await ctx.AddCardToHand<StrikeHexaghost>(), ctx.Combat.HittableEnemies.First()); // one ignition (prime only)
        Assert.IsTrue(Soulburn() - before > 0, "The prime ignites Searing.");
        Assert.AreEqual(2 * (Soulburn() - before), twoIgnitions, "Echo + prime = two ignitions.");
    }

    // Afterlife effects run without a CardPlay, so the card is never seen as the block source and
    // enchantments like Nimble (Fresnel Lens) used to be dropped from the Block gained on exhaust.
    [CardTest(typeof(Hexaghost.HexaghostCode.Core.Hexaghost))]
    public async Task AfterlifeBlockIncludesEnchantmentBonus(TestContext ctx)
    {
        var card = await ctx.AddCardToHand<PowerFromBeyond>();
        CardCmd.Enchant<Nimble>(card, 2);
        ctx.Player.Creature.LoseBlockInternal(ctx.Player.Creature.Block);

        await CardCmdCompatibility.Exhaust(new BlockingPlayerChoiceContext(), card, true);

        Assert.AreEqual(card.DynamicVars.Block.BaseValue + 2, (decimal)ctx.Player.Creature.Block,
            "Afterlife Block on exhaust should include Nimble's +2.");
    }

    [CardTest(typeof(Hexaghost.HexaghostCode.Core.Hexaghost))]
    public async Task AfterlifeBlockWithoutEnchantmentIsUnchanged(TestContext ctx)
    {
        var card = await ctx.AddCardToHand<PowerFromBeyond>();
        ctx.Player.Creature.LoseBlockInternal(ctx.Player.Creature.Block);

        await CardCmdCompatibility.Exhaust(new BlockingPlayerChoiceContext(), card, true);

        Assert.AreEqual(card.DynamicVars.Block.BaseValue, (decimal)ctx.Player.Creature.Block,
            "Afterlife Block on exhaust without enchantment should be the base Block.");
    }

    // Vambrace only doubles block that has a source card; Afterlife block must count as card block.
    [CardTest(typeof(Hexaghost.HexaghostCode.Core.Hexaghost))]
    public async Task AfterlifeBlockTriggersVambrace(TestContext ctx)
    {
        await RelicCmd.Obtain(ModelDb.Relic<Vambrace>().ToMutable(), ctx.Player);
        var card = await ctx.AddCardToHand<PowerFromBeyond>();
        ctx.Player.Creature.LoseBlockInternal(ctx.Player.Creature.Block);

        await CardCmdCompatibility.Exhaust(new BlockingPlayerChoiceContext(), card, true);

        Assert.AreEqual(card.DynamicVars.Block.BaseValue * 2, (decimal)ctx.Player.Creature.Block,
            "Vambrace should double the first card Block, including Afterlife Block on exhaust.");
    }

    // Afterlife attacks on exhaust get the same synthetic CardPlay, so damage enchantments (Sharp) must apply.
    [CardTest(typeof(Hexaghost.HexaghostCode.Core.Hexaghost))]
    public async Task AfterlifeAttackIncludesEnchantmentBonus(TestContext ctx)
    {
        var card = await ctx.AddCardToHand<SpectersWail>();
        CardCmd.Enchant<Sharp>(card, 3);
        var enemy = ctx.Combat.HittableEnemies.First();
        var hpBefore = enemy.CurrentHp;

        await CardCmdCompatibility.Exhaust(new BlockingPlayerChoiceContext(), card, true);

        Assert.AreEqual(card.DynamicVars.Damage.BaseValue + 3, (decimal)(hpBefore - enemy.CurrentHp),
            "Afterlife damage on exhaust should include Sharp's +3.");
    }

    [CardTest(typeof(Hexaghost.HexaghostCode.Core.Hexaghost))]
    public async Task SizzleExhaustsOnlyTheNextCardPlayed(TestContext ctx)
    {
        var sizzle = await ctx.AddCardToHand<Sizzle>();
        var next = await ctx.AddCardToHand<AdvancingGuard>();
        var after = await ctx.AddCardToHand<AdvancingGuard>();

        await ctx.PlayCard(sizzle);
        Assert.IsTrue(sizzle.Pile?.Type != PileType.Exhaust, "Sizzle itself must not be exhausted by its own power.");

        await ctx.PlayCard(next);
        Assert.IsTrue(next.Pile?.Type == PileType.Exhaust, "The next card played after Sizzle should be exhausted.");
        Assert.IsTrue(ctx.Player.Creature.GetPower<SizzlePower>() == null, "Sizzle power should be used up.");

        await ctx.PlayCard(after);
        Assert.IsTrue(after.Pile?.Type != PileType.Exhaust, "Only one card should be exhausted.");
    }

    // Regression: a replayed Sizzle used to treat its own second play as "the next card" and exhaust itself.
    [CardTest(typeof(Hexaghost.HexaghostCode.Core.Hexaghost))]
    public async Task ReplayedSizzleDoesNotExhaustItself(TestContext ctx)
    {
        var creature = ctx.Player.Creature;
        await PowerCmd.Apply<DuplicationPower>(new BlockingPlayerChoiceContext(), creature, 1, creature, null);
        var sizzle = await ctx.AddCardToHand<Sizzle>();
        var next = await ctx.AddCardToHand<AdvancingGuard>();

        await ctx.PlayCard(sizzle);

        Assert.IsTrue(sizzle.Pile?.Type != PileType.Exhaust, "A replayed Sizzle must not exhaust itself.");
        Assert.AreEqual(2m, (decimal)creature.GetPower<SizzlePower>()!.Amount,
            "Both plays of Sizzle should stack the power.");

        await ctx.PlayCard(next);
        Assert.IsTrue(next.Pile?.Type == PileType.Exhaust, "The next card should still be exhausted after a replayed Sizzle.");
    }

    // Card text says "whenever another player attacks this enemy": the applier's own attacks must not
    // trigger it, a teammate's must, and Soul Burn is credited to the attacker.
    [CardTest(typeof(Hexaghost.HexaghostCode.Core.Hexaghost), playerCount: 2)]
    public async Task BroilingFlamesTriggersOnlyOnOtherPlayersAttacks(TestContext ctx)
    {
        var teammate = ctx.Players[1];
        var enemy = ctx.Combat.HittableEnemies.First();
        await ctx.PlayCard(await ctx.AddCardToHand<BroilingFlames>(), enemy);
        var power = enemy.GetPower<BroilingFlamesPower>();
        Assert.IsTrue(power != null, "Broiling Flames should be applied to the enemy.");
        Assert.IsTrue(power!.TypeForCurrentAmount == PowerType.Debuff, "Broiling Flames sits on an enemy and should be a debuff.");

        await ctx.PlayCard(await ctx.AddCardToHand<StrikeHexaghost>(), enemy);
        Assert.IsTrue(enemy.GetPower<SoulBurnPower>() == null, "The applier's own attack must not trigger Broiling Flames.");

        await ctx.PlayCard(await ctx.AddCardToHand<StrikeHexaghost>(teammate), enemy);
        Assert.AreEqual(power.Amount, enemy.GetPower<SoulBurnPower>()?.Amount ?? 0, "A teammate's attack should apply Soul Burn once.");
    }

    // Each player's Broiling Flames is its own instance: with both on the enemy, each player's attack
    // triggers only the OTHER player's copy.
    [CardTest(typeof(Hexaghost.HexaghostCode.Core.Hexaghost), playerCount: 2)]
    public async Task BroilingFlamesIsInstancedPerApplier(TestContext ctx)
    {
        var teammate = ctx.Players[1];
        var enemy = ctx.Combat.HittableEnemies.First();
        await ctx.PlayCard(await ctx.AddCardToHand<BroilingFlames>(), enemy);
        await ctx.PlayCard(await ctx.AddCardToHand<BroilingFlames>(teammate), enemy);
        var copies = enemy.Powers.OfType<BroilingFlamesPower>().ToList();
        Assert.AreEqual(2, copies.Count, "Each player's Broiling Flames should be a separate instance.");

        await ctx.PlayCard(await ctx.AddCardToHand<StrikeHexaghost>(), enemy);
        var amount = copies[0].Amount;
        Assert.AreEqual(amount, enemy.GetPower<SoulBurnPower>()?.Amount ?? 0,
            "Player 1's attack should trigger only player 2's copy.");
    }

    // Cauterize's hit count comes from the captured X value and the Soul Burn it applies equals the damage dealt.
    // A multiplayer desync showed host/client disagreeing on both, so pin the relationship down in one place.
    [CardTest(typeof(Hexaghost.HexaghostCode.Core.Hexaghost))]
    public async Task CauterizeHitsXTimesAndAppliesDamageDealtAsSoulBurn(TestContext ctx)
    {
        await PlayerCmd.SetEnergy(3, ctx.Player);
        var enemy = ctx.Combat.HittableEnemies.First();
        var hpBefore = enemy.CurrentHp;
        var card = await ctx.AddCardToHand<Cauterize>();

        await ctx.PlayCard(card, enemy);

        var dealt = hpBefore - enemy.CurrentHp;
        var soulBurn = enemy.GetPower<SoulBurnPower>();
        Assert.AreEqual(3 * card.DynamicVars.Damage.IntValue, dealt, "Cauterize should hit once per energy held (AutoPlay captures X from current energy).");
        Assert.IsTrue(soulBurn != null, "Cauterize should apply Soul Burn.");
        Assert.AreEqual(dealt, soulBurn!.Amount, "Soul Burn should equal the HP lost (block/vulnerable-free target).");
    }

    // Cmd.Wait no-ops in tests, so poll on Task.Delay to let the enemy-turn state machine reach our next turn.
    private static async Task EndTurnAndWaitForNextTurn(TestContext ctx)
    {
        var startingTurn = ctx.Player.PlayerCombatState!.TurnNumber;
        PlayerCmd.EndTurn(ctx.Player, false);
        for (var i = 0; i < 50 && ctx.Player.PlayerCombatState!.TurnNumber == startingTurn; i++)
            await Task.Delay(100);
        Assert.IsTrue(ctx.Player.PlayerCombatState!.TurnNumber > startingTurn, "Next turn should have started.");
    }

    // Ethereal exhaust at turn end runs after Plating's payout, so Floatwork resolves early (IHasEarlyAfterlifeEffect)
    // and must apply Plating exactly once: 3 now, 2 after next turn's decay.
    [CardTest(typeof(Hexaghost.HexaghostCode.Core.Hexaghost))]
    public async Task FloatworkEtherealExhaustAtTurnEndAppliesPlatingOnce(TestContext ctx)
    {
        await ctx.AddCardToHand<Floatwork>();

        await EndTurnAndWaitForNextTurn(ctx);

        Assert.AreEqual(2, ctx.Player.Creature.GetPowerAmount<PlatingPower>(),
            "Plating should be applied once (3) and decay once (-1) by the next turn.");
    }

    [CardTest(typeof(Hexaghost.HexaghostCode.Core.Hexaghost))]
    public async Task FloatworkEtherealExhaustAtTurnEndStacksOnExistingPlating(TestContext ctx)
    {
        var creature = ctx.Player.Creature;
        await PowerCmd.Apply<PlatingPower>(new BlockingPlayerChoiceContext(), creature, 1, creature, null);
        await ctx.AddCardToHand<Floatwork>();

        await EndTurnAndWaitForNextTurn(ctx);

        Assert.AreEqual(3, creature.GetPowerAmount<PlatingPower>(), "1 + 3 Plating, minus 1 decay.");
    }

    // A non-turn-end exhaust is not early-resolved and must still trigger Afterlife.
    [CardTest(typeof(Hexaghost.HexaghostCode.Core.Hexaghost))]
    public async Task FloatworkExhaustedMidTurnAppliesPlating(TestContext ctx)
    {
        var card = await ctx.AddCardToHand<Floatwork>();

        await CardCmdCompatibility.Exhaust(new BlockingPlayerChoiceContext(), card, false);

        Assert.AreEqual(3, ctx.Player.Creature.GetPowerAmount<PlatingPower>(), "Afterlife should apply 3 Plating.");
    }

    // Flames ignite after the card resolves, but a card counts the flame its own play will ignite ("primed"), and the
    // displayed (calculated) count must equal what it actually uses.
    [CardTest(typeof(Hexaghost.HexaghostCode.Core.Hexaghost))]
    public async Task DividerCountsFlameItsOwnPlayPrimes(TestContext ctx)
    {
        var wheel = HexaghostCmd.GetWheel(ctx.Player);
        wheel[1].IsIgnited = true;
        var current = HexaghostCmd.GetCurrentFlame(ctx.Player);
        current.SetIgniteProgress(); // an Attack now ignites the current flame
        var divider = await ctx.AddCardToHand<Divider>();

        Assert.AreEqual(2m, divider.DynamicVars["Hits"].Calculate(null), "The ignited flame and the primed one count.");
        Assert.IsTrue(!(current.IsIgnited), "Priming must not ignite the flame early.");

        await ctx.PlayCard(divider, ctx.Combat.HittableEnemies.First());

        Assert.IsTrue(current.IsIgnited, "The flame should ignite after the card resolved.");
        Assert.AreEqual(2, HexaghostCmd.GetIgnitedCount(ctx.Player), "Both flames are ignited after the play.");
    }

    [CardTest(typeof(Hexaghost.HexaghostCode.Core.Hexaghost))]
    public async Task DividerDoesNotCountFlameItsPlayDoesNotPrime(TestContext ctx)
    {
        HexaghostCmd.GetWheel(ctx.Player)[1].IsIgnited = true;
        var divider = await ctx.AddCardToHand<Divider>();

        // Searing needs 2 attacks and none was played yet, so this Attack only progresses it.
        Assert.AreEqual(1m, divider.DynamicVars["Hits"].Calculate(null), "Only the already ignited flame counts.");
    }

    [CardTest(typeof(Hexaghost.HexaghostCode.Core.Hexaghost))]
    public async Task HauntingEchoSeesItsOwnPrimedFlame(TestContext ctx)
    {
        var current = HexaghostCmd.GetCurrentFlame(ctx.Player);
        var echo = await ctx.AddCardToHand<HauntingEcho>();
        Assert.IsTrue(!(HexaghostCmd.IsIgnited(ctx.Player, echo)), "Not primed while the flame needs more attacks.");

        current.SetIgniteProgress();
        Assert.IsTrue(HexaghostCmd.IsIgnited(ctx.Player, echo), "The Attack would ignite the flame, so it is primed.");
        Assert.IsTrue(!(current.IsIgnited), "Priming must not ignite the flame early.");

        await ctx.PlayCard(echo, ctx.Combat.HittableEnemies.First());

        Assert.IsTrue(current.IsIgnited, "The flame is ignited after the play, and only once.");
    }

    // A Skill never primes the attack-triggered flame, however full it is.
    [CardTest(typeof(Hexaghost.HexaghostCode.Core.Hexaghost))]
    public async Task SkillDoesNotPrimeAnAttackFlame(TestContext ctx)
    {
        HexaghostCmd.GetCurrentFlame(ctx.Player).SetIgniteProgress();
        var step = await ctx.AddCardToHand<StepThrough>(); // an Attack: primes
        var defend = await ctx.AddCardToHand<DefendHexaghost>();

        Assert.AreEqual(1m, step.DynamicVars["Ignited"].Calculate(null), "An Attack primes the Attack flame.");
        Assert.IsTrue(!(HexaghostCmd.GetCurrentFlame(ctx.Player).IsPrimedBy(defend)), "A Skill does not.");
    }

    // The "Hits" text must preview what the attack will really do, including hit-count modifiers (Thumbscrew on
    // newer game builds). The plain case runs everywhere; the Thumbscrew case only where that relic exists.
    [CardTest(typeof(Hexaghost.HexaghostCode.Core.Hexaghost))]
    public async Task DividerHitsPreviewMatchesCalculatedWithoutModifiers(TestContext ctx)
    {
        HexaghostCmd.GetWheel(ctx.Player)[1].IsIgnited = true;
        HexaghostCmd.GetWheel(ctx.Player)[2].IsIgnited = true;
        var divider = await ctx.AddCardToHand<Divider>();

        var hits = divider.DynamicVars["Hits"];
        hits.UpdateCardPreview(divider, CardPreviewMode.Normal, null, true);

        Assert.AreEqual(2m, hits.PreviewValue, "With no hit-count modifier the preview equals the calculated hits.");
    }

    [CardTest(typeof(Hexaghost.HexaghostCode.Core.Hexaghost))]
    public async Task DividerHitsPreviewIncludesThumbscrew(TestContext ctx)
    {
        // Looked up by name: Thumbscrew only exists on newer game builds, and referencing the type would stop this
        // whole test class from loading on older ones.
        var thumbscrew = ModelDb.AllRelics.FirstOrDefault(r => r.GetType().Name == "Thumbscrew");
        if (thumbscrew == null) return;

        await RelicCmd.Obtain(thumbscrew.ToMutable(), ctx.Player);
        HexaghostCmd.GetWheel(ctx.Player)[1].IsIgnited = true;
        HexaghostCmd.GetWheel(ctx.Player)[2].IsIgnited = true;
        var divider = await ctx.AddCardToHand<Divider>();

        var hits = divider.DynamicVars["Hits"];
        hits.UpdateCardPreview(divider, CardPreviewMode.Normal, null, true);

        Assert.AreEqual(3m, hits.PreviewValue, "Thumbscrew adds one hit to an attack that already hits more than once.");
    }
}
