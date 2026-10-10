using Downfall.DownfallCode.Tests;
using Downfall.DownfallCode.Powers;
using Hexaghost.HexaghostCode.Cards.Common;
using Hexaghost.HexaghostCode.Cards.Rare;
using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models;

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
