using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using SlimeBoss.SlimeBossCode.Cards.Common;
using SlimeBoss.SlimeBossCode.Cards.Rare;
using SlimeBoss.SlimeBossCode.Cards.Token;
using SlimeBoss.SlimeBossCode.Cards.Uncommon;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Powers;
using SlimeBoss.SlimeBossCode.Slimes;

namespace Downfall.TestCode;

public class SlimeBossTests
{
    // Regression guard: Duplicated Form checked `cardPlay.Target?.Side == CombatSide.Enemy` to decide
    // whether a card play targeted an enemy, but AoE cards (TargetType.AllEnemies) are played with a
    // null Target - there's no single selected creature - so that check always failed for them and
    // Duplicated Form silently never doubled AoE cards. The fix resolves the card's actual targets via
    // DownfallCmd.TargetsEnemy (CardModel.MyGetTargets) instead of trusting the raw Target.
    [CardTest(typeof(SlimeBoss.SlimeBossCode.Core.SlimeBoss))]
    public async Task DuplicatedFormDoublesAoeCardsTargetingEnemies(TestContext ctx)
    {
        var enemy = ctx.Combat.HittableEnemies.First();
        await PowerCmd.Apply<DuplicatedFormPower>(new BlockingPlayerChoiceContext(), ctx.Player.Creature, 1,
            ctx.Player.Creature, null);

        var rollThrough = await ctx.AddCardToHand<RollThrough>();
        await ctx.PlayCard(rollThrough, null);

        Assert.AreEqual(2, enemy.GetPower<WeakPower>()?.Amount ?? 0,
            "Duplicated Form should double an AllEnemies card even though it's played with a null target.");
    }

    // Equalize: "Deal damage. Consume - Gain Block equal to damage dealt." Consume now keys off the
    // target's Weak stacks (SlimeBossCmd.Consume), not the old Goop-bonus-on-attack pipeline.
    [CardTest(typeof(SlimeBoss.SlimeBossCode.Core.SlimeBoss))]
    public async Task EqualizeConsumeGrantsBlockEqualToDamageDealt(TestContext ctx)
    {
        var enemy = ctx.Combat.HittableEnemies.First();
        var lick = await ctx.AddCardToHand<Lick>();
        await ctx.PlayCard(lick, enemy);

        var equalize = await ctx.AddCardToHand<Equalize>();
        var startingBlock = ctx.Player.Creature.Block;

        await ctx.PlayCard(equalize, enemy);

        var damageDealt = ctx.Player.Creature.Block - startingBlock;
        Assert.IsTrue(damageDealt >= 12,
            $"Equalize should Consume the Weak from Lick and gain Block equal to its damage dealt (>=12), got a delta of {damageDealt}.");
    }

    // Without Weak on the target, Equalize's Consume shouldn't trigger at all - no Block gained.
    [CardTest(typeof(SlimeBoss.SlimeBossCode.Core.SlimeBoss))]
    public async Task EqualizeGrantsNoBlockWithoutWeakOnTarget(TestContext ctx)
    {
        var enemy = ctx.Combat.HittableEnemies.First();
        var equalize = await ctx.AddCardToHand<Equalize>();
        var startingBlock = ctx.Player.Creature.Block;

        await ctx.PlayCard(equalize, enemy);

        Assert.AreEqual(startingBlock, ctx.Player.Creature.Block,
            "Equalize's Consume should not trigger (no Block gained) when the target has no Weak.");
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
}
