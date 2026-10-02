using Hexaghost.HexaghostCode.Cards.Common;
using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.TestCode;

public class HexaghostTests
{
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
}
