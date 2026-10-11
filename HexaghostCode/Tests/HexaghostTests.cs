using Downfall.DownfallCode.Compatibility;
using Downfall.DownfallCode.Tests;
using Downfall.DownfallCode.Powers;
using Hexaghost.HexaghostCode.Cards.Common;
using Hexaghost.HexaghostCode.Cards.Rare;
using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;
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
}
