using Collector.CollectorCode.Cards.Basic;
using Collector.CollectorCode.Cards.Common;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Cards.Uncommon;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Extensions;
using Collector.CollectorCode.Intents;
using Collector.CollectorCode.Interfaces;
using Collector.CollectorCode.Powers;
using Downfall.DownfallCode.Tests;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Collector.CollectorCode.Tests;

public class CollectorTests
{
    // Regression guard (standalone-mod smoke test, issue 04): confirms Collector's own standalone
    // assembly/manifest still registers and plays normally - i.e. the Collector.csproj extraction
    // into a genuinely separate mod didn't break anything.
    [CardTest(typeof(Collector.CollectorCode.Core.Collector))]
    public IEnumerable<CardTestCase> PlayCollectorCards(CharacterModel character) => AllCardsTest.PlayAllCards(character);

    // Reserve conversion is for X-energy cards only; an X-star card pays its (numeric) energy cost normally.
    [CardTest(typeof(Collector.CollectorCode.Core.Collector))]
    public async Task XStarCardDoesNotConvertReserve(TestContext ctx)
    {
        var card = await ctx.AddCardToHand<Stardust>();
        ctx.Player.PlayerCombatState!.Energy = 2;
        await ReserveCmd.GainReserve(ctx.Player, 3);

        await card.SpendResources();

        Assert.AreEqual(3, ctx.Player.PlayerCombatState.Reserve, "An X-star card must leave Reserve untouched.");
    }

    [CardTest(typeof(Collector.CollectorCode.Core.Collector))]
    public async Task PyreCardWithOtherHandCardIsPlayable(TestContext ctx)
    {
        await ctx.ClearHand();
        var roast = await ctx.AddCardToHand<Roast>();
        await ctx.AddCardToHand<FuelTheFire>();

        Assert.IsTrue(roast.CanPlay(), "Roast should be playable once another card is in hand to exhaust.");
    }


    // Regression guard for a previously-missing feature: X-cost cards only spent Energy and never
    // touched Reserve, so Collector's Reserve resource did nothing to boost their effect. CardModel.
    // SpendResources() is the only place that actually deducts Energy/sets CapturedXValue (CardCmd.
    // AutoPlay, which TestContext.PlayCard uses, plays cards "for free" and never calls it), so this
    // calls it directly rather than going through ctx.PlayCard.
    [CardTest]
    public async Task XCostCardSpendsEnergyAndReserve(TestContext ctx)
    {
        var card = await ctx.AddCardToHand<Whirlwind>();
        ctx.Player.PlayerCombatState!.Energy = 2;
        await ReserveCmd.GainReserve(ctx.Player, 3);

        var (energySpent, _) = await card.SpendResources();

        Assert.AreEqual(5, energySpent, "X-cost card should report Energy + Reserve as spent.");
        Assert.AreEqual(5, card.EnergyCost.CapturedXValue, "X value should be Energy + Reserve.");
        Assert.AreEqual(0, ctx.Player.PlayerCombatState.Energy, "All Energy should be spent.");
        Assert.AreEqual(0, ctx.Player.PlayerCombatState.Reserve, "All Reserve should be spent.");
    }

    [CardTest]
    public async Task XCostCardWithNoReserveOnlySpendsEnergy(TestContext ctx)
    {
        var card = await ctx.AddCardToHand<Whirlwind>();
        ctx.Player.PlayerCombatState!.Energy = 3;

        var (energySpent, _) = await card.SpendResources();

        Assert.AreEqual(3, energySpent, "With no Reserve, X-cost card should only spend Energy.");
        Assert.AreEqual(3, card.EnergyCost.CapturedXValue, "X value should equal Energy when Reserve is empty.");
        Assert.AreEqual(0, ctx.Player.PlayerCombatState.Energy, "All Energy should be spent.");
        Assert.AreEqual(0, ctx.Player.PlayerCombatState.Reserve, "Reserve should remain empty.");
    }

    // Regression guard for the existing (non-X-cost) behavior: Reserve should still only cover the
    // Energy deficit, unaffected by the X-cost handling added above.
    [CardTest(typeof(Collector.CollectorCode.Core.Collector))]
    public async Task NormalCostCardStillUsesReserveOnlyToCoverDeficit(TestContext ctx)
    {
        var card = await ctx.AddCardToHand<BidingBlast>();
        ctx.Player.PlayerCombatState!.Energy = 0;
        await ReserveCmd.GainReserve(ctx.Player, 5);

        var (energySpent, _) = await card.SpendResources();

        Assert.AreEqual(1, energySpent, "BidingBlast costs 1.");
        Assert.AreEqual(0, ctx.Player.PlayerCombatState.Energy, "Energy was already empty.");
        Assert.AreEqual(4, ctx.Player.PlayerCombatState.Reserve, "Only the 1-cost deficit should be covered by Reserve.");
    }

    // Regression guard: CollectorEnergy.ShouldPlay used to re-check Energy+Reserve affordability on top of
    // CheckResources, which also gates CardCmd.AutoPlay - the base game's "play this card for free" path
    // (used by echo/replay/duplicate effects, and TestContext.PlayCard). AutoPlay never spends resources, so
    // gating it on affordability silently ate free plays of cards the player couldn't otherwise afford, and
    // surfaced the CollectorEnergy singleton as an unrecognized "preventer" to the base game's
    // UnplayableReason.GetPlayerDialogueLine switch, which only knows Card/Relic/Power/Enchantment/Affliction
    // models and logs an ERROR for anything else.
    [CardTest(typeof(Collector.CollectorCode.Core.Collector))]
    public async Task AutoPlayIsNotBlockedByInsufficientEnergyOrReserve(TestContext ctx)
    {
        var enemy = ctx.Combat.HittableEnemies.First();
        var startHp = enemy.CurrentHp;
        var card = await ctx.AddCardToHand<Collector.CollectorCode.Cards.Common.SuckerPunch>(); // costs 2
        ctx.Player.PlayerCombatState!.Energy = 0;
        await ReserveCmd.GainReserve(ctx.Player, 1); // Energy(0) + Reserve(1) < cost(2), and Reserve > 0

        await ctx.PlayCard(card, enemy);

        // AutoPlay moves the card out of Hand into a result pile whether it actually played or was blocked
        // (MoveToResultPileWithoutPlaying), so check its actual effect (damage dealt) rather than its pile.
        Assert.IsTrue(enemy.CurrentHp < startHp,
            "AutoPlay should still play (and deal damage from) a card the player can't afford, since it's a free play.");
    }

    // Regression guard for ReturnToHandAfterTurnEndPatch: the game hardcodes moving a HasTurnEndInHandEffect
    // card to Discard once its turn-end effect resolves (CombatManager.ResolveTurnEndCardEffects), so without
    // the patch Ember would end its turn in Discard instead of Hand despite implementing IReturnsToHandAfterTurnEnd.
    [CardTest]
    public async Task EmberReturnsToHandInsteadOfDiscardAfterTurnEnd(TestContext ctx)
    {
        var ember = await ctx.AddCardToHand<Ember>();
        var startingHp = ctx.Player.Creature.CurrentHp;

        PlayerCmd.EndTurn(ctx.Player, false);
        await Cmd.Wait(1f);

        Assert.AreEqual(PileType.Hand, ember.Pile?.Type,
            "Ember should return to Hand after its turn-end effect, not be discarded.");
        Assert.IsTrue(ctx.Player.Creature.CurrentHp < startingHp,
            "Ember's turn-end effect should still deal its self-damage.");
    }

    // Regression guard: Torchhead auto-attacks from TorchheadPower.AfterSideTurnEnd instead of acting
    // through the normal monster move state machine, so its NextMove is never rolled by the enemy turn
    // loop unless TorchheadCmd.RefreshTorchheadIntent does it manually - without that call, the intent
    // icon stays blank. Also checks the displayed value is post-power (Weak), not just the base amount.
    [CardTest(typeof(Collector.CollectorCode.Core.Collector))]
    public async Task TorchheadIntentShowsCurrentAndPowerModifiedDamage(TestContext ctx)
    {
        var torchhead = await TorchheadCmd.Kindle(new BlockingPlayerChoiceContext(), ctx.Player, 10, null);

        var intent = torchhead.Monster?.NextMove.Intents.FirstOrDefault() as TorchheadAttackIntent;
        Assert.IsTrue(intent != null,
            "Torchhead's move state should carry a TorchheadAttackIntent so its intent icon shows.");

        var baseLabel = intent!.GetIntentLabel(ctx.Combat.Players.Select(p => p.Creature), torchhead).GetFormattedText();
        Assert.IsTrue(baseLabel.Contains("5"), $"Intent should show the base 5 damage, got '{baseLabel}'.");

        await PowerCmd.Apply<WeakPower>(new BlockingPlayerChoiceContext(), torchhead, 1, ctx.Player.Creature, null);
        var weakenedLabel = intent.GetIntentLabel(ctx.Combat.Players.Select(p => p.Creature), torchhead).GetFormattedText();

        Assert.IsTrue(weakenedLabel.Contains("3"),
            $"Weak should reduce Torchhead's displayed intent damage from 5 to 3, got '{weakenedLabel}'.");
    }

    // Regression guard: Torchhead's intent description should say whether it hits all enemies or
    // just the lowest-HP one (CollectorHook.ShouldTorchheadTargetAll), since that's a real gameplay
    // difference (e.g. EquipAxePower) that was previously invisible in the intent tooltip.
    [CardTest(typeof(Collector.CollectorCode.Core.Collector))]
    public async Task TorchheadIntentDescriptionReflectsTargetingMode(TestContext ctx)
    {
        var torchhead = await TorchheadCmd.Kindle(new BlockingPlayerChoiceContext(), ctx.Player, 10, null);
        var intent = torchhead.Monster?.NextMove.Intents.FirstOrDefault() as TorchheadAttackIntent;
        Assert.IsTrue(intent != null, "Torchhead's move state should carry a TorchheadAttackIntent.");

        var targets = ctx.Combat.Players.Select(p => p.Creature).ToList();
        var singleTargetDescription = intent!.GetHoverTip(targets, torchhead).Description;
        Assert.IsTrue(singleTargetDescription.Contains("least HP"),
            $"Without EquipAxe, Torchhead should target the enemy with the least HP, got '{singleTargetDescription}'.");

        await PowerCmd.Apply<EquipAxePower>(new BlockingPlayerChoiceContext(), ctx.Player.Creature, 1, ctx.Player.Creature, null);
        var allTargetsDescription = intent.GetHoverTip(targets, torchhead).Description;

        Assert.IsTrue(allTargetsDescription.Contains("ALL enemies"),
            $"With EquipAxe, Torchhead should target all enemies, got '{allTargetsDescription}'.");
    }

    // Regression guard: CollectorCardModel injects "TorchheadTargetsAll" for every Collector card, so a
    // Torchhead-attack card's text flips to "to ALL enemies" once EquipAxe is active without per-card overrides.
    [CardTest(typeof(Collector.CollectorCode.Core.Collector))]
    public async Task TorchheadCardDescriptionReflectsTargetingMode(TestContext ctx)
    {
        await ctx.ClearHand();
        var card = await ctx.AddCardToHand<Collector.CollectorCode.Cards.Common.AshenStrike>();
        Assert.IsTrue(!card.GetDescriptionForPile(PileType.Hand).Contains("ALL enemies"),
            "Without EquipAxe, the card description should not say ALL enemies.");

        await PowerCmd.Apply<EquipAxePower>(new BlockingPlayerChoiceContext(), ctx.Player.Creature, 1, ctx.Player.Creature, null);
        Assert.IsTrue(card.GetDescriptionForPile(PileType.Hand).Contains("ALL enemies"),
            "With EquipAxe, the card description should say ALL enemies.");
    }

    // Regression guard: InevitableDemisePower's IModifyCollectorMiasmaIncrement only ever gets
    // consulted from inside MiasmaPower's own end-of-turn trigger, which never runs without an
    // existing Miasma instance - so the debuff used to do nothing at all against an enemy with no
    // Miasma yet. It should grant a single stack directly instead.
    [CardTest(typeof(Collector.CollectorCode.Core.Collector))]
    public async Task InevitableDemiseGrantsMiasmaWhenTargetHasNone(TestContext ctx)
    {
        var enemy = ctx.Combat.HittableEnemies.First();
        await PowerCmd.Apply<InevitableDemisePower>(new BlockingPlayerChoiceContext(), enemy, 1, ctx.Player.Creature, null);
        Assert.IsTrue(!enemy.HasPower<MiasmaPower>(), "Setup: enemy should start without Miasma.");

        // Cmd.Wait is a no-op under TestMode, so it never yields back to the engine's frame loop
        // that drives the enemy-turn state machine - poll on the turn counter (a real Task.Delay
        // does yield) until AfterSideTurnEnd has actually had a chance to run.
        var startingTurn = ctx.Player.PlayerCombatState!.TurnNumber;
        PlayerCmd.EndTurn(ctx.Player, false);
        for (var i = 0; i < 50 && ctx.Player.PlayerCombatState!.TurnNumber == startingTurn; i++)
            await Task.Delay(100);

        Assert.IsTrue(enemy.HasPower<MiasmaPower>(),
            "InevitableDemise should grant Miasma when the enemy has none, instead of doing nothing.");
        Assert.AreEqual(1, enemy.GetInstancedPowerAmountSum<MiasmaPower>(),
            "Should grant exactly 1 Miasma as a fallback.");
    }

    // Regression guard for Pyre state on replayed plays: cards pyre inline in their own OnPlayInternal
    // (see PyreCmd.Pyre) and store the pyred card(s) on the card instance (IUsesPyredCards.PyredCards),
    // clearing them once done, so every replay (OnPlayWrapper's playCount loop) must pyre its own card
    // and end with no leftover state.
    [CardTest(typeof(Collector.CollectorCode.Core.Collector))]
    public async Task ReplayedPyreCardPyresOncePerPlayAndLeavesNoState(TestContext ctx)
    {
        await ctx.ClearHand();
        var enemy = ctx.Combat.HittableEnemies.First();
        var startHp = enemy.CurrentHp;
        var lash = await ctx.AddCardToHand<FlameLash>();
        var strikeA = await ctx.AddCardToHand<StrikeIronclad>();
        var strikeB = await ctx.AddCardToHand<StrikeIronclad>();
        await PowerCmd.Apply<DuplicationPower>(new BlockingPlayerChoiceContext(), ctx.Player.Creature, 1, ctx.Player.Creature, null);

        await ctx.PlayCard(lash, enemy);

        Assert.AreEqual(PileType.Exhaust, strikeA.Pile?.Type, "First play should pyre the first hand card.");
        Assert.AreEqual(PileType.Exhaust, strikeB.Pile?.Type, "The replay should pyre its own (second) hand card.");
        Assert.AreEqual(16, startHp - enemy.CurrentHp, "Both plays should deal FlameLash damage (8 x 2).");
        Assert.IsTrue(!((IUsesPyredCards)lash).PyredCards.Any(), "PyredCards should be empty after the play finished.");
    }

    // A replay whose Pyre can't be paid (no other hand card left) must be cancelled, and must not reuse the
    // previous play's pyred card.
    [CardTest(typeof(Collector.CollectorCode.Core.Collector))]
    public async Task ReplayedPyreCardIsCancelledWhenNothingLeftToPyre(TestContext ctx)
    {
        await ctx.ClearHand();
        var enemy = ctx.Combat.HittableEnemies.First();
        var startHp = enemy.CurrentHp;
        var lash = await ctx.AddCardToHand<FlameLash>();
        var strike = await ctx.AddCardToHand<StrikeIronclad>();
        await PowerCmd.Apply<DuplicationPower>(new BlockingPlayerChoiceContext(), ctx.Player.Creature, 1, ctx.Player.Creature, null);

        await ctx.PlayCard(lash, enemy);

        Assert.AreEqual(PileType.Exhaust, strike.Pile?.Type, "The only other card should be pyred by the first play.");
        Assert.AreEqual(8, startHp - enemy.CurrentHp, "Only the first play should resolve; the replay has nothing to pyre.");
        Assert.IsTrue(!((IUsesPyredCards)lash).PyredCards.Any(), "PyredCards should not keep the first play's card.");
    }

    // Reserve-only cards (IUsesCollectorEnergyOnly) are paid entirely from Reserve and never touch Energy.
    [CardTest(typeof(Collector.CollectorCode.Core.Collector))]
    public async Task ReserveOnlyCardSpendsOnlyReserve(TestContext ctx)
    {
        var card = await ctx.AddCardToHand<Collector.CollectorCode.Cards.Rare.FingerOfDeath>(); // costs 4
        ctx.Player.PlayerCombatState!.Energy = 3;
        await ReserveCmd.GainReserve(ctx.Player, 6);

        var (energySpent, _) = await card.SpendResources();

        Assert.AreEqual(0, energySpent, "A Reserve-only card should not spend Energy.");
        Assert.AreEqual(3, ctx.Player.PlayerCombatState.Energy, "Energy should be untouched.");
        Assert.AreEqual(2, ctx.Player.PlayerCombatState.Reserve, "The full 4 cost should come out of Reserve.");
    }

    // Affordability: Reserve-only cards ignore Energy; ordinary cards can be paid by Energy + Reserve combined.
    [CardTest(typeof(Collector.CollectorCode.Core.Collector))]
    public async Task ReserveOnlyCardPlayabilityIgnoresEnergy(TestContext ctx)
    {
        await ctx.ClearHand();
        var card = await ctx.AddCardToHand<Collector.CollectorCode.Cards.Rare.FingerOfDeath>(); // costs 4
        ctx.Player.PlayerCombatState!.Energy = 10;
        await ReserveCmd.GainReserve(ctx.Player, 3);
        Assert.IsTrue(!card.CanPlay(), "Reserve 3 < cost 4, so a Reserve-only card is unplayable however much Energy there is.");

        await ReserveCmd.GainReserve(ctx.Player, 1);
        Assert.IsTrue(card.CanPlay(), "Reserve 4 >= cost 4, so the card should be playable.");
    }

    [CardTest(typeof(Collector.CollectorCode.Core.Collector))]
    public async Task OrdinaryCardPlayabilityCombinesEnergyAndReserve(TestContext ctx)
    {
        await ctx.ClearHand();
        var card = await ctx.AddCardToHand<Collector.CollectorCode.Cards.Common.SuckerPunch>(); // costs 2
        ctx.Player.PlayerCombatState!.Energy = 1;
        Assert.IsTrue(!card.CanPlay(), "Energy 1 + Reserve 0 < cost 2.");

        await ReserveCmd.GainReserve(ctx.Player, 1);
        Assert.IsTrue(card.CanPlay(), "Energy 1 + Reserve 1 covers cost 2.");
    }
}
