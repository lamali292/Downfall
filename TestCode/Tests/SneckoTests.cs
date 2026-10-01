using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Relics;
using Snecko.SneckoCode.Cards.Basic;
using Snecko.SneckoCode.Cards.Common;
using Snecko.SneckoCode.Cards.Rare;
using Snecko.SneckoCode.Cards.Uncommon;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.Powers;
using Snecko.SneckoCode.Relics;

namespace Downfall.TestCode;

public class SneckoTests
{
    // Cost-module consistency: X-energy and X-star cards have no numeric cost, so every Muddle path
    // must skip them the same way. Muddle selection prompts auto-pick the first eligible card, so the
    // X card goes first in hand: if it were eligible it would be the one muddled.
    [CardTest(typeof(Snecko.SneckoCode.Core.Snecko))]
    public async Task MuddleSelectionSkipsXStarCard(TestContext ctx)
    {
        await ctx.ClearHand();
        var stardust = await ctx.AddCardToHand<Stardust>();
        var strike = await ctx.AddCardToHand<StrikeIronclad>();
        var snekBite = await ctx.AddCardToHand<SnekBite>();

        await ctx.PlayCard(snekBite, ctx.Combat.HittableEnemies.First());

        Assert.IsTrue(!stardust.EnergyCost.HasLocalModifiers, "Muddle must not target an X-star card.");
        Assert.IsTrue(strike.EnergyCost.HasLocalModifiers, "Muddle should have targeted the numeric card instead.");
    }

    [CardTest(typeof(Snecko.SneckoCode.Core.Snecko))]
    public async Task MuddleSelectionSkipsXEnergyCard(TestContext ctx)
    {
        await ctx.ClearHand();
        var whirlwind = await ctx.AddCardToHand<Whirlwind>();
        var strike = await ctx.AddCardToHand<StrikeIronclad>();
        var snekBite = await ctx.AddCardToHand<SnekBite>();

        await ctx.PlayCard(snekBite, ctx.Combat.HittableEnemies.First());

        Assert.IsTrue(!whirlwind.EnergyCost.HasLocalModifiers, "Muddle must not target an X-energy card.");
        Assert.IsTrue(strike.EnergyCost.HasLocalModifiers, "Muddle should have targeted the numeric card instead.");
    }

    // Reroll used to take Max() over an empty sequence (throws) when the hand held only X cards.
    [CardTest(typeof(Snecko.SneckoCode.Core.Snecko))]
    public async Task RerollWithOnlyXCardsInHandDoesNothing(TestContext ctx)
    {
        await ctx.ClearHand();
        var whirlwind = await ctx.AddCardToHand<Whirlwind>();
        var stardust = await ctx.AddCardToHand<Stardust>();
        var reroll = await ctx.AddCardToHand<Reroll>();

        await ctx.PlayCard(reroll);

        Assert.IsTrue(!whirlwind.EnergyCost.HasLocalModifiers, "Reroll must not muddle an X-energy card.");
        Assert.IsTrue(!stardust.EnergyCost.HasLocalModifiers, "Reroll must not muddle an X-star card.");
    }

    [CardTest(typeof(Snecko.SneckoCode.Core.Snecko))]
    public async Task CheapStockSkipsXCards(TestContext ctx)
    {
        await ctx.ClearHand();
        var stardust = await ctx.AddCardToHand<Stardust>();
        var whirlwind = await ctx.AddCardToHand<Whirlwind>();
        var strike = await ctx.AddCardToHand<StrikeIronclad>();
        var power = await PowerCmd.Apply<CheapStockPower>(new BlockingPlayerChoiceContext(), ctx.Player.Creature, 3,
            ctx.Player.Creature, null);

        await power!.AfterSideTurnStart(ctx.Player.Creature.Side, ctx.Combat.Creatures, ctx.Combat);

        Assert.IsTrue(!stardust.EnergyCost.HasLocalModifiers, "Cheap Stock must not muddle an X-star card.");
        Assert.IsTrue(!whirlwind.EnergyCost.HasLocalModifiers, "Cheap Stock must not muddle an X-energy card.");
        Assert.IsTrue(strike.EnergyCost.HasLocalModifiers, "Cheap Stock should muddle the numeric card.");
    }

    // Shed gives block per card that ends up free. An X card's base cost is stored as 0 but it is not free.
    [CardTest(typeof(Snecko.SneckoCode.Core.Snecko))]
    public async Task ShedDoesNotCountXCardsAsFree(TestContext ctx)
    {
        await ctx.ClearHand();
        await ctx.AddCardToHand<Whirlwind>();
        var shed = await ctx.AddCardToHand<Shed>();
        var blockBefore = ctx.Player.Creature.Block;

        await ctx.PlayCard(shed);

        Assert.AreEqual(blockBefore, ctx.Player.Creature.Block, "An X card must not count as zero-cost for Shed.");
    }

    // Gift's cost filter reads the printed cost, so a temporary discount doesn't change what matches.
    [CardTest(typeof(Snecko.SneckoCode.Core.Snecko))]
    public async Task GiftMinCostUsesPrintedCost(TestContext ctx)
    {
        var strike = await ctx.AddCardToHand<StrikeIronclad>();
        strike.EnergyCost.SetThisTurn(0);

        Assert.IsTrue(new Gift { MinCost = 1 }.Matches(strike), "Gift MinCost should use the printed cost, not the discounted one.");
    }

    // Gift's own tooltip says it "gets a card reward", so reward-modifying relics like Silver
    // Crucible must see Gift's candidates the same way they'd see a normal card reward screen.
    [CardTest(typeof(Snecko.SneckoCode.Core.Snecko))]
    public async Task GiftCardIsUpgradedBySilverCrucible(TestContext ctx)
    {
        await RelicCmd.Obtain<SilverCrucible>(ctx.Player);

        var before = PileType.Deck.GetPile(ctx.Player).Cards.ToList();
        await SneckoCmd.GetGift(ctx.Player, new Gift { Rarity = CardRarity.Common });
        var added = PileType.Deck.GetPile(ctx.Player).Cards.Except(before).ToList();

        Assert.AreEqual(1, added.Count, "Gift should add exactly one card to the deck.");
        Assert.IsTrue(added[0].IsUpgraded, "Silver Crucible should upgrade the card obtained through a Gift.");
    }

    // Regression guard: without a reward-modifying relic in play, Gift shouldn't upgrade cards
    // on its own (the CardReward plumbing must not force an upgrade by itself).
    [CardTest(typeof(Snecko.SneckoCode.Core.Snecko))]
    public async Task GiftCardIsNotUpgradedWithoutSilverCrucible(TestContext ctx)
    {
        var before = PileType.Deck.GetPile(ctx.Player).Cards.ToList();
        await SneckoCmd.GetGift(ctx.Player, new Gift { Rarity = CardRarity.Common });
        var added = PileType.Deck.GetPile(ctx.Player).Cards.Except(before).ToList();

        Assert.AreEqual(1, added.Count, "Gift should add exactly one card to the deck.");
        Assert.IsTrue(!added[0].IsUpgraded, "Without Silver Crucible, Gift shouldn't upgrade the obtained card.");
    }

    // Beyond Armor doesn't draw (it puts a specific Offclass card from the draw pile into hand,
    // per its own wording), so it shouldn't interact with draw hooks/relics like Fiddle at all.
    [CardTest(typeof(Snecko.SneckoCode.Core.Snecko))]
    public async Task BeyondArmorPutsOffclassCardIntoHand(TestContext ctx)
    {
        var offclass = await ctx.AddCardToTopOfDraw<StrikeIronclad>();
        var beyondArmor = await ctx.AddCardToHand<BeyondArmor>();
        await ctx.PlayCard(beyondArmor);

        Assert.IsTrue(ctx.Player.Hand.Contains(offclass), "Beyond Armor should put the Offclass card into hand.");
    }

    // Reported bug: Serpent Idol put its picked card into hand via a plain CardPileCmd.Add, which
    // never fires AfterCardGeneratedForCombat - card-creation triggers like Pillar of Creation
    // (and, before an earlier fix, Arsenal for Unending Supply) never saw the card as "generated".
    [CardTest(typeof(Snecko.SneckoCode.Core.Snecko))]
    public async Task SerpentIdolTriggersPillarOfCreation(TestContext ctx)
    {
        await PowerCmd.Apply<PillarOfCreationPower>(new BlockingPlayerChoiceContext(), ctx.Player.Creature, 1,
            ctx.Player.Creature, null);
        var serpentIdol = await ctx.AddCardToHand<SerpentIdol>();
        var blockBefore = ctx.Player.Creature.Block;

        await ctx.PlayCard(serpentIdol);

        Assert.IsTrue(ctx.Player.Creature.Block > blockBefore,
            "Pillar of Creation should trigger off the card Serpent Idol puts into hand.");
    }

    // Reported bug: SneckoModel.AfterRoomEntered (act 1 floor 1 pool-character selection) can fire
    // twice if another mod re-triggers the same room-entered hook, which used to reserve a second
    // batch of choice ids and run the whole 3-round selection again, granting 6 SneckoChoice relics
    // instead of 3. NChooseACardSelectionScreen.ShowScreen returns null under TestMode.IsOn, which
    // GetLocalChoice treats as "pick index 0" - so this runs deterministically without a real screen.
    [CardTest(typeof(Snecko.SneckoCode.Core.Snecko))]
    public async Task RunActEntryOnlyRunsOnceEvenIfCalledTwice(TestContext ctx)
    {
        var runState = ctx.Player.RunState;

        SneckoPoolSelection.RunActEntry(runState);
        SneckoPoolSelection.RunActEntry(runState);

        var sw = System.Diagnostics.Stopwatch.StartNew();
        while (ctx.Player.Relics.OfType<SneckoChoice>().Count() < 3 && sw.Elapsed < TimeSpan.FromSeconds(5))
            await Task.Yield();
        // Give a second run, if one were wrongly kicked off, a chance to add its extra relics too.
        await Task.Delay(200);

        var count = ctx.Player.Relics.OfType<SneckoChoice>().Count();
        Assert.AreEqual(3, count,
            $"RunActEntry should only run once even when called twice for the same run, got {count} SneckoChoice relics.");
    }

    // Regression guard for save-quit-and-reload specifically: reloading deserializes a brand new
    // IRunState (a fresh key for the in-memory HasRunActEntry gate), but the player's SneckoChoice
    // relics from before the save are still there. This test starts a player with all 3 already
    // obtained - as if just reloaded post-selection - and its own [CardTest] combat setup naturally
    // gives it a never-before-seen IRunState, so the in-memory gate alone would NOT catch a re-run
    // here; only the per-player relic-count filter does.
    [CardTest(typeof(Snecko.SneckoCode.Core.Snecko))]
    public async Task RunActEntrySkipsPlayerWhoAlreadyHasFullSneckoChoiceSetFromBeforeAReload(TestContext ctx)
    {
        for (var i = 0; i < 3; i++)
        {
            var relic = (SneckoChoice)ModelDb.Relic<SneckoChoice>().ToMutable();
            relic.InitCharacter(ModelDb.Character<Ironclad>());
            await RelicCmd.Obtain(relic, ctx.Player);
        }

        Assert.AreEqual(3, ctx.Player.Relics.OfType<SneckoChoice>().Count(),
            "Sanity check: player should start this test with exactly 3 SneckoChoice relics.");

        SneckoPoolSelection.RunActEntry(ctx.Player.RunState);
        await Task.Delay(200);

        var count = ctx.Player.Relics.OfType<SneckoChoice>().Count();
        Assert.AreEqual(3, count,
            $"RunActEntry should skip a player who already has a full SneckoChoice set (e.g. after a " +
            $"save/reload), got {count} SneckoChoice relics.");
    }

    // "Super Snecko" mode: owning Prismatic Snecko should make GetSneckoCharacterModels/GetSneckoCards
    // return every character's pool at once, not just the ones individually picked via SneckoChoice.
    [CardTest(typeof(Snecko.SneckoCode.Core.Snecko))]
    public async Task PrismaticSneckoGrantsEveryCharactersCardPool(TestContext ctx)
    {
        await RelicCmd.Obtain(ModelDb.Relic<PrismaticSnecko>().ToMutable(), ctx.Player);

        var chars = SneckoModel.GetSneckoCharacterModels(ctx.Player).ToList();
        var expected = ModelDb.AllCharacters.Where(c => c != ctx.Player.Character).ToList();

        Assert.AreEqual(expected.Count, chars.Count,
            $"Prismatic Snecko should grant every other character's pool, got {chars.Count} of {expected.Count}.");
        Assert.IsTrue(expected.All(chars.Contains), "Prismatic Snecko's pool should include every other character.");
    }

    // Reload-safety mirror of RunActEntrySkipsPlayerWhoAlreadyHasFullSneckoChoiceSetFromBeforeAReload,
    // but for a player who reached "done" via Prismatic Snecko instead of 3 individual SneckoChoice
    // picks - IsDoneSelecting (ISneckoPoolSupplier.ActEntryWeight) must recognize both as complete.
    [CardTest(typeof(Snecko.SneckoCode.Core.Snecko))]
    public async Task RunActEntrySkipsPlayerWhoAlreadyHasPrismaticSnecko(TestContext ctx)
    {
        await RelicCmd.Obtain(ModelDb.Relic<PrismaticSnecko>().ToMutable(), ctx.Player);

        SneckoPoolSelection.RunActEntry(ctx.Player.RunState);
        await Task.Delay(200);

        Assert.AreEqual(1, ctx.Player.Relics.OfType<PrismaticSnecko>().Count(),
            "RunActEntry should not grant a second Prismatic Snecko.");
        Assert.AreEqual(0, ctx.Player.Relics.OfType<SneckoChoice>().Count(),
            "RunActEntry should not also run the normal picker for a player who already has Prismatic Snecko.");
    }

    private static async Task PlayShapeshift(TestContext ctx, bool upgraded = false)
    {
        var shapeshift = await ctx.AddCardToHand<Shapeshift>();
        if (upgraded) CardCmd.Upgrade(shapeshift);
        await ctx.PlayCard(shapeshift);
    }

    // Basic cards keep working: they turn into another character's Basic card.
    [CardTest(typeof(Snecko.SneckoCode.Core.Snecko))]
    public async Task ShapeshiftTransformsBasicCards(TestContext ctx)
    {
        await ctx.ClearHand();
        var strike = await ctx.AddCardToHand<StrikeIronclad>();
        var defend = await ctx.AddCardToHand<DefendIronclad>();
        await PlayShapeshift(ctx);

        var hand = ctx.Player.Hand.ToList();
        Assert.IsTrue(!hand.Contains(strike) && !hand.Contains(defend), "Basic cards should have been transformed.");
        Assert.AreEqual(2, hand.Count, $"Hand should still hold two cards, got {hand.Count}.");
        Assert.IsTrue(hand.All(c => c.Rarity == CardRarity.Basic), "Basic cards should stay Basic.");
        Assert.IsTrue(hand.All(c => c.Id != strike.Id && c.Id != defend.Id), "No card should transform into its own kind.");
    }

    // Common/Uncommon/Rare cards keep their rarity and never turn into themselves.
    [CardTest(typeof(Snecko.SneckoCode.Core.Snecko))]
    public async Task ShapeshiftKeepsRarityOfCommonUncommonAndRare(TestContext ctx)
    {
        await ctx.ClearHand();
        var originals = new[]
        {
            await ctx.AddCardToHand<Anger>(),
            await ctx.AddCardToHand<Accuracy>(),
            await ctx.AddCardToHand<Adrenaline>()
        };
        var rarities = originals.Select(c => c.Rarity).ToList();
        await PlayShapeshift(ctx);

        var hand = ctx.Player.Hand.ToList();
        Assert.AreEqual(3, hand.Count, $"Hand should still hold three cards, got {hand.Count}.");
        Assert.IsTrue(originals.All(o => !hand.Contains(o)), "Every card should have been transformed.");
        Assert.IsTrue(originals.All(o => hand.All(c => c.Id != o.Id)), "No card should transform into its own kind.");
        Assert.IsTrue(rarities.OrderBy(r => r).SequenceEqual(hand.Select(c => c.Rarity).OrderBy(r => r)),
            "Rarities should be preserved across the transformation.");
    }

    // Ancient cards are replaced by another Ancient card; an Event card (no same-rarity candidates in the borrowed
    // pools) is left alone. Neither may break the bulk transformation.
    [CardTest(typeof(Snecko.SneckoCode.Core.Snecko))]
    public async Task ShapeshiftHandlesAncientAndEventCards(TestContext ctx)
    {
        await ctx.ClearHand();
        var ancient = await ctx.AddCardToHand<Apotheosis>();
        var eventCard = await ctx.AddCardToHand<Clash>();
        Assert.IsTrue(ancient.IsTransformable, "Test setup: the Ancient card in hand should be transformable.");
        await PlayShapeshift(ctx);

        var hand = ctx.Player.Hand.ToList();
        Assert.AreEqual(2, hand.Count, $"Hand should still hold two cards, got {hand.Count}.");
        Assert.IsTrue(!hand.Contains(ancient), "The Ancient card should be replaced.");
        Assert.IsTrue(hand.Any(c => c.Rarity == CardRarity.Ancient && c.Id != ancient.Id),
            "An Ancient card should turn into another Ancient card.");
        Assert.IsTrue(hand.Contains(eventCard) || hand.Any(c => c.Rarity == CardRarity.Event),
            "The Event card should stay Event (replaced by an Event card or left alone).");
    }

    // Overflow is decided from the hand at play start and the decision belongs to that one play: a second
    // play of the same kind with a small hand must not inherit the first play's "active" decision.
    [CardTest(typeof(Snecko.SneckoCode.Core.Snecko))]
    public async Task OverflowFiresOnlyForThePlayThatStartedWithAFullHand(TestContext ctx)
    {
        await ctx.ClearHand();
        var enemy = ctx.Combat.HittableEnemies.First();
        var fullHandWhip = await ctx.AddCardToHand<TailWhip>();
        for (var i = 0; i < 5; i++) await ctx.AddCardToHand<StrikeIronclad>();

        await ctx.PlayCard(fullHandWhip, enemy);
        var weakAfterFullHand = enemy.GetInstancedPowerAmountSum<WeakPower>();
        Assert.IsTrue(weakAfterFullHand > 0, "Overflow should apply Weak when the play starts with 5 other cards in hand.");

        await ctx.ClearHand();
        var smallHandWhip = await ctx.AddCardToHand<TailWhip>();
        await ctx.PlayCard(smallHandWhip, enemy);
        Assert.AreEqual(weakAfterFullHand, enemy.GetInstancedPowerAmountSum<WeakPower>(),
            "A later play with a small hand must not fire Overflow again.");
    }

    [CardTest(typeof(Snecko.SneckoCode.Core.Snecko))]
    public async Task ShapeshiftPlusUpgradesTheReplacements(TestContext ctx)
    {
        await ctx.ClearHand();
        await ctx.AddCardToHand<Anger>();
        await ctx.AddCardToHand<Accuracy>();
        await PlayShapeshift(ctx, upgraded: true);

        var hand = ctx.Player.Hand.ToList();
        Assert.AreEqual(2, hand.Count, $"Hand should still hold two cards, got {hand.Count}.");
        Assert.IsTrue(hand.All(c => c.IsUpgraded || !c.IsUpgradable), "Shapeshift+ should upgrade the replacements.");
    }
}
