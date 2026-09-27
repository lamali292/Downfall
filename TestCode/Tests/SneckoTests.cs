using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Relics;
using Snecko.SneckoCode.Cards.Common;
using Snecko.SneckoCode.Cards.Uncommon;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.Relics;

namespace Downfall.TestCode;

public class SneckoTests
{
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
            relic.InitCharacter(ModelDb.Get<Ironclad>());
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
}
