using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Relics;
using Snecko.SneckoCode.Cards.Common;
using Snecko.SneckoCode.Cards.Uncommon;
using Snecko.SneckoCode.Core;

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
}
