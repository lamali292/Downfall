using Awakened.AwakenedCode.Cards.Basic;
using Awakened.AwakenedCode.Cards.Common;
using Awakened.AwakenedCode.Cards.Rare;
using Awakened.AwakenedCode.Cards.Uncommon;
using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Powers;
using BaseLib.Extensions;
using Downfall.TestCode;
using MegaCrit.Sts2.Core.AutoSlay;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Relics;

namespace Awakened.AwakenedCode.Tests;

public class AwakenedTests
{
    // Regression guard (internal-submod smoke test): confirms Awakened's own standalone assembly
    // still registers and plays normally with no replacement mod loaded - i.e. the Awakened.csproj
    // extraction and ReplaceableSubmod guard didn't break anything.
    [CardTest(typeof(Awakened.AwakenedCode.Core.Awakened))]
    public IEnumerable<CardTestCase> PlayAwakenedCards(CharacterModel character) => AllCardsTest.PlayAllCards(character);

    // Clutch counts cards whose modified cost is exactly zero; an unplayable negative-cost card
    // (Ascender's Bane) must not look free.
    [CardTest(typeof(Awakened.AwakenedCode.Core.Awakened))]
    public async Task ClutchIgnoresNegativeCostCards(TestContext ctx)
    {
        var bane = await ctx.AddCardToTopOfDraw<AscendersBane>();
        var clutch = await ctx.AddCardToHand<Clutch>();
        await ctx.PlayCard(clutch, ctx.Combat.HittableEnemies.First());

        Assert.IsTrue(!ctx.Player.Hand.Contains(bane), "Clutch must not fetch a negative-cost card as if it were free.");
    }
    private static int ZeroCostCardsInHand(TestContext ctx) =>
        ctx.Player.Hand.Count(c => c.EnergyCost.GetAmountToSpend() == 0 && !c.EnergyCost.CostsX);

    // Clutch doesn't draw (it puts a random 0-cost card from the draw pile into hand, per its own
    // wording), so it shouldn't interact with draw hooks/relics like Fiddle at all. The pick is
    // random and Awakened's own starting deck already has a 0-cost card (Hymn), so this asserts on
    // the count rather than a specific seeded instance - checking for one exact card would be flaky.
    [CardTest(typeof(Awakened.AwakenedCode.Core.Awakened))]
    public async Task ClutchPutsZeroCostCardIntoHand(TestContext ctx)
    {
        await ctx.AddCardToTopOfDraw<Hymn>();
        var clutch = await ctx.AddCardToHand<Clutch>();
        var before = ZeroCostCardsInHand(ctx);

        await ctx.PlayCard(clutch, ctx.Combat.HittableEnemies.First());

        Assert.AreEqual(before + 1, ZeroCostCardsInHand(ctx), "Clutch should put exactly one 0-cost card into hand.");
    }

    // Regression guard: a card that Snecko Eye randomized to a non-zero cost must keep that cost
    // (SetThisCombat persists on the CardModel instance regardless of pile) after being discarded
    // and reshuffled back into the draw pile, and Clutch must respect it - reported as "Clutch
    // brought me this 2-cost Blunder Guard" after a Snecko Eye reshuffle.
    [CardTest(typeof(Awakened.AwakenedCode.Core.Awakened))]
    public async Task ClutchDoesNotPickCardsRandomizedNonZeroBySneckoEye(TestContext ctx)
    {
        var sneckoEye = await RelicCmd.Obtain<SneckoEye>(ctx.Player);
        sneckoEye.SetTestEnergyCostOverride(2);

        var hymn = await ctx.AddCardToTopOfDraw<Hymn>();
        var drawnCard = await CardPileCmd.Draw(new BlockingPlayerChoiceContext(), ctx.Player);
        AutoSlayLog.Info($"[AwakenedTests] drawn={drawnCard == hymn} cost={hymn.EnergyCost.GetAmountToSpend()}");
        Assert.AreEqual(hymn, drawnCard, "Sanity check: should have drawn the seeded Hymn.");
        Assert.AreEqual(2, hymn.EnergyCost.GetAmountToSpend(), "Snecko Eye should have randomized Hymn's cost to 2.");

        await CardCmd.Discard(new BlockingPlayerChoiceContext(), hymn);
        await CardPileCmd.Shuffle(new BlockingPlayerChoiceContext(), ctx.Player);
        AutoSlayLog.Info($"[AwakenedTests] backInDraw={ctx.Player.DrawPile.Contains(hymn)} costAfterShuffle={hymn.EnergyCost.GetAmountToSpend()}");
        Assert.IsTrue(ctx.Player.DrawPile.Contains(hymn), "Sanity check: Hymn should be back in the draw pile after reshuffle.");
        Assert.AreEqual(2, hymn.EnergyCost.GetAmountToSpend(), "Sanity check: Hymn's randomized cost should survive the reshuffle.");

        var clutch = await ctx.AddCardToHand<Clutch>();
        await ctx.PlayCard(clutch, ctx.Combat.HittableEnemies.First());

        AutoSlayLog.Info($"[AwakenedTests] hymnInHand={ctx.Player.Hand.Contains(hymn)}");
        Assert.IsTrue(!ctx.Player.Hand.Contains(hymn),
            "Clutch should not pick a card that Snecko Eye randomized to a non-zero cost.");
    }

    // Regression guard: reported by a player - Vigor's damage bonus was only applying to Recitation's
    // base attack, not to the extra attack triggered by Chant, even though both hits are shown on the
    // card and should each get the bonus. Root cause was that the chant used a second, separate
    // AttackCommand - Vigor is consumed after one AttackCommand.Execute(), so only the first hit got
    // it. Fixed by dealing both hits from a single AttackCommand (hitCount: 2) when chanting.
    // HasChanted is set directly (rather than playing a Power first) so this isolates Vigor's
    // per-hit behavior from unrelated turn state.
    [CardTest(typeof(Awakened.AwakenedCode.Core.Awakened))]
    public async Task VigorAppliesToBothRecitationHitsWhenChanted(TestContext ctx)
    {
        var enemy = ctx.Combat.HittableEnemies.First();
        var startHp = enemy.CurrentHp;

        await PowerCmd.Apply<VigorPower>(new BlockingPlayerChoiceContext(), ctx.Player.Creature, 5,
            ctx.Player.Creature, null);

        var recitation = (Recitation)await ctx.AddCardToHand<Recitation>();
        ChantCmd.SetChanted(recitation);
        await ctx.PlayCard(recitation, enemy);

        var totalDamage = startHp - enemy.CurrentHp;
        Assert.AreEqual(22, totalDamage,
            "Vigor should boost both the base attack and the chant-triggered attack from Recitation " +
            "((6 base + 5 vigor) * 2 hits = 22).");
    }

    // Regression guard: Rising Chorus gated its double-trigger on the chanted card's own
    // "firstTime" flag (whether that specific card instance had ever chanted before), instead of
    // on whether this was the turn's first chant activation. A card that had already chanted once
    // - from an earlier turn, or from being drawn again with HasChanted still set on the instance -
    // would never get doubled again, even on the first chant of a brand new turn.
    [CardTest(typeof(Awakened.AwakenedCode.Core.Awakened))]
    public async Task RisingChorusDoublesChantOfAnAlreadyChantedCard(TestContext ctx)
    {
        await PowerCmd.Apply<RisingChorusPower>(new BlockingPlayerChoiceContext(), ctx.Player.Creature, 1,
            ctx.Player.Creature, null);

        var featherFlare = (FeatherFlare)await ctx.AddCardToHand<FeatherFlare>();
        ChantCmd.SetChanted(featherFlare);

        await ctx.PlayCard(featherFlare, ctx.Combat.HittableEnemies.First());

        var drawPower = ctx.Player.Creature.Powers.OfType<DrawCardsNextTurnPower>().FirstOrDefault();
        Assert.IsTrue(drawPower != null && drawPower.Amount == 2,
            "Rising Chorus should double an already-chanted card's chant effect on the turn's first chant " +
            $"(expected DrawCardsNextTurnPower amount 2, got {drawPower?.Amount.ToString() ?? "none"}).");
    }

    // Regression guard for the IModifyChantRepeatCount refactor: ChantCmd.Chant only asks for a
    // repeat count on a genuine chant (isFirstChantInSeries) and runs every resulting bonus chant with
    // that flag false, so a listener can never recurse into itself through the loop. A Rising Chorus
    // Amount far larger than ChantThisTurn would, without that guard, have the bonus chant's own
    // ModifyChantRepeatCount call see ChantThisTurn (still 1) <= Amount and keep granting more bonus
    // chants forever. This asserts the chant only ever doubles (not triples+) even at high Amount.
    [CardTest(typeof(Awakened.AwakenedCode.Core.Awakened))]
    public async Task RisingChorusDoesNotRecurseBeyondOneBonusChantAtHighAmount(TestContext ctx)
    {
        await PowerCmd.Apply<RisingChorusPower>(new BlockingPlayerChoiceContext(), ctx.Player.Creature, 10,
            ctx.Player.Creature, null);

        var featherFlare = (FeatherFlare)await ctx.AddCardToHand<FeatherFlare>();
        ChantCmd.SetChanted(featherFlare);
        await ctx.PlayCard(featherFlare, ctx.Combat.HittableEnemies.First());

        var drawPower = ctx.Player.Creature.Powers.OfType<DrawCardsNextTurnPower>().FirstOrDefault();
        Assert.IsTrue(drawPower != null && drawPower.Amount == 2,
            "A single chant should trigger exactly one bonus chant (not recurse further) regardless of " +
            $"how high Rising Chorus's Amount is, got DrawCardsNextTurnPower amount={drawPower?.Amount.ToString() ?? "none"}.");
    }

    // Regression guard: ChantCmd.Chant now queries ModifyChantRepeatCount before recording this
    // chant's own ChantEntry, so RisingChorusPower's ChantThisTurn (counted from history) only sees
    // chants strictly before the current one - it must compare with "<" rather than "<=" to still mean
    // "only the turn's first Amount chants get doubled". With the wrong operator, a second chant this
    // turn would see ChantThisTurn==1<=Amount(1) and incorrectly double too.
    [CardTest(typeof(Awakened.AwakenedCode.Core.Awakened))]
    public async Task RisingChorusOnlyDoublesTheTurnsFirstChantNotTheSecond(TestContext ctx)
    {
        await PowerCmd.Apply<RisingChorusPower>(new BlockingPlayerChoiceContext(), ctx.Player.Creature, 1,
            ctx.Player.Creature, null);

        var first = (FeatherFlare)await ctx.AddCardToHand<FeatherFlare>();
        ChantCmd.SetChanted(first);
        var second = (FeatherFlare)await ctx.AddCardToHand<FeatherFlare>();
        ChantCmd.SetChanted(second);

        await ctx.PlayCard(first, ctx.Combat.HittableEnemies.First());
        await ctx.PlayCard(second, ctx.Combat.HittableEnemies.First());

        var drawPower = ctx.Player.Creature.Powers.OfType<DrawCardsNextTurnPower>().FirstOrDefault();
        Assert.IsTrue(drawPower != null && drawPower.Amount == 3,
            "Only the turn's first chant should double (first +2, second +1 = 3 total), got " +
            $"DrawCardsNextTurnPower amount={drawPower?.Amount.ToString() ?? "none"}.");
    }

    // Regression guard: GreatHex's and Victuals' own chant effect briefly called ChantCmd.Chant
    // again from inside PlayChantEffect (instead of from OnPlayInternal) - since Chant() re-checks its
    // own gate (which is already true once HasChanted is set) that was unconditional self-recursion, a
    // guaranteed stack overflow the moment either card actually chanted. This plays each one with
    // HasChanted forced true (so the chant fires) and asserts the chant effect actually lands exactly
    // once, which both proves there's no infinite recursion and that the effect still works at all now
    // that nothing but the card's own OnPlayInternal triggers its chant.
    [CardTest(typeof(Awakened.AwakenedCode.Core.Awakened))]
    public async Task GreatHexAppliesItsPowerOnceWhenChanted(TestContext ctx)
    {
        var greatHex = (GreatHex)await ctx.AddCardToHand<GreatHex>();
        ChantCmd.SetChanted(greatHex);
        var enemy = ctx.Combat.HittableEnemies.First();

        await ctx.PlayCard(greatHex, enemy);

        var amount = enemy.GetPower<GreatHexPower>()?.Amount ?? 0;
        Assert.AreEqual((int)greatHex.DynamicVars.Power<GreatHexPower>().BaseValue, amount,
            $"GreatHex should apply its GreatHexPower exactly once when chanted, got Amount={amount}.");
    }

    [CardTest(typeof(Awakened.AwakenedCode.Core.Awakened))]
    public async Task VictualsGainsEnergyOnceWhenChanted(TestContext ctx)
    {
        var victuals = (Victuals)await ctx.AddCardToHand<Victuals>();
        ChantCmd.SetChanted(victuals);
        var energyBefore = ctx.Player.PlayerCombatState!.Energy;

        await ctx.PlayCard(victuals);

        Assert.AreEqual(energyBefore + (int)victuals.DynamicVars.Energy.BaseValue, ctx.Player.PlayerCombatState!.Energy,
            "Victuals should gain its chant energy exactly once.");
    }

    // Regression guard: reported for offclass Byrd's Eye - it read spellbook.Cards without
    // refilling first, so once the spellbook was fully emptied (e.g. every base spell already
    // conjured away) there was nothing to select and the card did nothing on play. Fixed by
    // refreshing the spellbook when empty, same fallback AwakenedCmd.ConjureSpell already uses.
    [CardTest(typeof(Awakened.AwakenedCode.Core.Awakened))]
    public async Task ByrdsEyeRefillsEmptySpellbookBeforeConjuring(TestContext ctx)
    {
        AwakenedCmd.InitSpellbook(ctx.Player);
        var spellbook = AwakenedCmd.GetSpellbook(ctx.Player);
        foreach (var card in spellbook.Cards.ToList())
            spellbook.RemoveInternal(card);
        Assert.AreEqual(0, spellbook.Cards.Count, "Sanity check: spellbook should be empty before playing Byrd's Eye.");

        var byrdsEye = await ctx.AddCardToHand<ByrdsEye>();
        var handCountBefore = ctx.Player.Hand.Count;

        await ctx.PlayCard(byrdsEye);

        Assert.AreEqual(handCountBefore, ctx.Player.Hand.Count,
            "Byrd's Eye should conjure a spell into hand even when the spellbook started empty " +
            "(hand count should stay the same: -1 for playing Byrd's Eye, +1 for the conjured spell).");
    }
}
