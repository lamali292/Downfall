using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Cards.Uncommon;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Extensions;
using Collector.CollectorCode.Intents;
using Collector.CollectorCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.TestCode;

public class CollectorTests
{
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
        await CollectorCmd.GainReserve(ctx.Player, 3);

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
        await CollectorCmd.GainReserve(ctx.Player, 5);

        var (energySpent, _) = await card.SpendResources();

        Assert.AreEqual(1, energySpent, "BidingBlast costs 1.");
        Assert.AreEqual(0, ctx.Player.PlayerCombatState.Energy, "Energy was already empty.");
        Assert.AreEqual(4, ctx.Player.PlayerCombatState.Reserve, "Only the 1-cost deficit should be covered by Reserve.");
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
    // loop unless CollectorCmd.RefreshTorchheadIntent does it manually - without that call, the intent
    // icon stays blank. Also checks the displayed value is post-power (Weak), not just the base amount.
    [CardTest(typeof(Collector.CollectorCode.Core.Collector))]
    public async Task TorchheadIntentShowsCurrentAndPowerModifiedDamage(TestContext ctx)
    {
        var torchhead = await CollectorCmd.Kindle(new BlockingPlayerChoiceContext(), ctx.Player, 10, null);

        var intent = torchhead.Monster.NextMove.Intents.FirstOrDefault() as TorchheadAttackIntent;
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
        var torchhead = await CollectorCmd.Kindle(new BlockingPlayerChoiceContext(), ctx.Player, 10, null);
        var intent = torchhead.Monster.NextMove.Intents.FirstOrDefault() as TorchheadAttackIntent;
        Assert.IsTrue(intent != null, "Torchhead's move state should carry a TorchheadAttackIntent.");

        var targets = ctx.Combat.Players.Select(p => p.Creature);
        var singleTargetDescription = intent!.GetHoverTip(targets, torchhead).Description;
        Assert.IsTrue(singleTargetDescription.Contains("least HP"),
            $"Without EquipAxe, Torchhead should target the enemy with the least HP, got '{singleTargetDescription}'.");

        await PowerCmd.Apply<EquipAxePower>(new BlockingPlayerChoiceContext(), ctx.Player.Creature, 1, ctx.Player.Creature, null);
        var allTargetsDescription = intent.GetHoverTip(targets, torchhead).Description;

        Assert.IsTrue(allTargetsDescription.Contains("ALL enemies"),
            $"With EquipAxe, Torchhead should target all enemies, got '{allTargetsDescription}'.");
    }
}
