using Automaton.AutomatonCode.Cards.Basic;
using Automaton.AutomatonCode.Cards.Common;
using Automaton.AutomatonCode.Cards.Rare;
using Automaton.AutomatonCode.Cards.Status;
using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.Cards.Uncommon;
using Automaton.AutomatonCode.Compile;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using Automaton.AutomatonCode.Extensions;
using Automaton.AutomatonCode.Powers;
using Automaton.AutomatonCode.Relics;
using BaseLib.Extensions;
using Downfall.DownfallCode.Compatibility;
using Downfall.DownfallCode.Tests;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Automaton.AutomatonCode.Tests;

public class AutomatonTests
{

    [CardTest(typeof(Core.Automaton))]
    public IEnumerable<CardTestCase> PlayAutomatonCards(CharacterModel character) => AllCardsTest.PlayAllCards(character);

    // Regression guard: MergeConflictPower.AfterCardGeneratedForCombat fires again for the
    // clone it adds via AddGeneratedCardToCombat, which used to re-trigger itself and cascade
    // into Amount copies from a single Function creation instead of ticking down by 1 per
    // creation and making exactly one copy each time.
    [CardTest(typeof(Automaton.AutomatonCode.Core.Automaton))]
    public async Task MergeConflictOnlyMakesOneCopyPerFunctionCreation(TestContext ctx)
    {
        var choiceCtx = new BlockingPlayerChoiceContext();
        await PowerCmd.Apply<MergeConflictPower>(choiceCtx, ctx.Player.Creature, 2, ctx.Player.Creature, null);

        for (var i = 0; i < AutomatonCmd.GetMax(ctx.Player); i++)
            await AutomatonCmd.EncodeCard<OilSpill>(ctx.Player, choiceCtx);

        var functionCount = ctx.Player.Hand.Count(c => c is FunctionCard);
        Assert.AreEqual(2, functionCount,
            "One Function creation with MergeConflict at 2 should yield exactly one extra copy (2 total), not cascade into more.");

        var remaining = ctx.Player.Creature.GetInstancedPowerAmountSum<MergeConflictPower>();
        Assert.AreEqual(1, remaining, "MergeConflict should tick down by exactly 1 per Function creation.");
    }

    // Regression guard: Full Release (PowerEncode) makes a Function skip its other Encodings on
    // play entirely (FunctionCard.OnPlay breaks right after PowerEncode) and instead release them
    // later through FullReleasePower, whose turn-start trigger passes no CardModel source and no
    // fixed target (the target enemy is re-rolled each turn). Class Default must NOT scale that
    // deferred Block/Damage: baking a snapshot in at grant time would also freeze in
    // target-dependent modifiers (Weak, Vulnerable, ...) against whatever enemy happened to be
    // resolved first, which is wrong once the power re-targets on later turns. Class Default only
    // affects Attack/Skill Functions (CardType.Power Functions never resolve Block/Damage through
    // a play at all - see FunctionCard.OnPlay), so a compiled Power Function's preview must not
    // show the bonus either, matching what actually happens when it's played.
    [CardTest(typeof(Automaton.AutomatonCode.Core.Automaton))]
    public async Task ClassDefaultDoesNotScaleFullReleaseDeferredBlock(TestContext ctx)
    {
        var choiceCtx = new BlockingPlayerChoiceContext();
        var classDefault =
            await PowerCmd.Apply<ClassDefaultPower>(choiceCtx, ctx.Player.Creature, 5, ctx.Player.Creature, null);

        // BronzeCore auto-encodes a Defend + Strike on turn 1 (AutomatonCode/Relics/BronzeCore.cs).
        // Play one filler Boost to complete and flush that batch before setting up the real one.
        await ctx.PlayCard(await ctx.AddCardToHand<Boost>());

        // Boost (Block + Strength) x2 and Full Release (Power) x1 compile into a fresh 3-card
        // Function that carries both Block and the Full Release Power effect.
        await ctx.PlayCard(await ctx.AddCardToHand<Boost>());
        await ctx.PlayCard(await ctx.AddCardToHand<Boost>());
        await ctx.PlayCard(await ctx.AddCardToHand<FullRelease>());

        var function = ctx.Player.Hand.OfType<FunctionCard>()
            .FirstOrDefault(f => f.DynamicVars.Power<FullReleasePower>().BaseValue > 0);
        Assert.IsTrue(function != null, "Boost x2 + Full Release should have compiled into a Function in hand.");
        Assert.IsTrue(function!.Type == CardType.Power, "A Function carrying Full Release should be CardType.Power.");
        var rawBlock = function.DynamicVars.Block.BaseValue;
        Assert.IsTrue(rawBlock > 0, "Function should carry the encoded Block from Boost.");

        var previewBonus = classDefault!.ModifyBlockAdditive(ctx.Player.Creature, rawBlock,
            function.DynamicVars.Block.Props, function, null);
        Assert.AreEqual(0m, previewBonus,
            "Class Default must not preview a Block bonus on a Power Function - it will never actually be delivered.");

        await ctx.PlayCard(function);

        var granted = ctx.Player.Creature.GetPowerInstances<FullReleasePower>().LastOrDefault();
        Assert.IsTrue(granted != null, "Playing the Function should grant a Full Release Power instance.");
        Assert.AreEqual(rawBlock, granted!.DynamicVars.Block.BaseValue,
            "Class Default must not scale the Block deferred through Full Release Power - only Block from a Function card actually being played.");
    }

    // Regression guard: vanilla Rebound only redirects/consumes a charge on a card that's about to
    // be Discarded (it already leaves Exhaust cards alone since those resolve to PileType.Exhaust
    // before Rebound ever looks at them). Encode isn't a vanilla pile concept, so an Encodable card
    // used to still resolve to Discard at that point - Rebound "helpfully" redirected it to the draw
    // pile and burned a charge, even though AutomatonCombatModel.AfterCardPlayed was
    // about to forcibly move the card into the Encode pile a moment later anyway.
    [CardTest(typeof(Automaton.AutomatonCode.Core.Automaton))]
    public async Task ReboundDoesNotConsumeChargeOnEncodedCard(TestContext ctx)
    {
        var choiceCtx = new BlockingPlayerChoiceContext();
        await PowerCmd.Apply<ReboundPower>(choiceCtx, ctx.Player.Creature, 1, ctx.Player.Creature, null);

        var card = await ctx.AddCardToHand<Boost>();
        await ctx.PlayCard(card);

        var remaining = ctx.Player.Creature.GetInstancedPowerAmountSum<ReboundPower>();
        Assert.AreEqual(1, remaining,
            "Rebound should not consume a charge when the played card is about to be Encoded instead of Discarded.");
    }

    // Companion to ReboundDoesNotConsumeChargeOnEncodedCard: makes sure the Encode-specific carve-out
    // didn't disable Rebound outright. A card that never self-encodes (StrikeAutomaton is only
    // player-Encodable when force-encoded, e.g. by Platinum Core) should still redirect to the draw
    // pile and consume a charge like vanilla intends.
    [CardTest(typeof(Automaton.AutomatonCode.Core.Automaton))]
    public async Task ReboundStillConsumesChargeOnNormalCard(TestContext ctx)
    {
        var choiceCtx = new BlockingPlayerChoiceContext();
        await PowerCmd.Apply<ReboundPower>(choiceCtx, ctx.Player.Creature, 1, ctx.Player.Creature, null);

        var target = ctx.Combat.HittableEnemies.First();
        var card = await ctx.AddCardToHand<StrikeAutomaton>();
        await ctx.PlayCard(card, target);

        var remaining = ctx.Player.Creature.GetInstancedPowerAmountSum<ReboundPower>();
        Assert.AreEqual(0, remaining,
            "Rebound should still consume its charge and redirect a normal (non-Encoded) card to the draw pile.");
    }

    // Regression guard: vanilla dupes (History Course, Feral, ...) always cease to exist after
    // playing instead of going anywhere - CardModel.GetResultLocationForCardPlay hardcodes
    // PileType.None for IsDupe cards specifically so they never linger in any pile. Encode used to
    // ignore that and stash the dupe into the Encode pile anyway (and even compile it into a
    // Function once the pile filled up), keeping a copy alive that vanilla intends to disappear.
    [CardTest(typeof(Automaton.AutomatonCode.Core.Automaton))]
    public async Task DupedEncodableCardDoesNotEndUpInEncodePile(TestContext ctx)
    {
        var choiceCtx = new BlockingPlayerChoiceContext();

        // BronzeCore auto-encodes a Defend + Strike on turn 1 (AutomatonCode/Relics/BronzeCore.cs);
        // play a filler card first so that unrelated batch is already resolved.
        await ctx.PlayCard(await ctx.AddCardToHand<Boost>());
        var beforeCount = ctx.Player.EncodePile.Count;

        var card = await ctx.AddCardToHand<Boost>();
        var dupe = card.CreateDupeCompat();
        await CardCmd.AutoPlay(choiceCtx, dupe, null);

        Assert.AreEqual(beforeCount, ctx.Player.EncodePile.Count,
            "A dupe of an Encodable card should not end up in the Encode pile - it should cease to exist like any other dupe.");
    }

    // Characterization: playing a Strike with Platinum Core Exhausts that exact card (via an
    // AfterCardPlayed call, after the card's play-result location - and therefore Rebound - has
    // already been resolved) and separately Encodes one Throw token. Rebound sees the
    // Strike heading to Discard like any other card and spends its charge on it before Platinum
    // Core pulls the card into the Exhaust pile instead.
    [CardTest(typeof(Automaton.AutomatonCode.Core.Automaton))]
    public async Task PlatinumCoreStrikeExhaustsCardAndEncodesOneToken(TestContext ctx)
    {
        var choiceCtx = new BlockingPlayerChoiceContext();
        await ctx.PlayCard(await ctx.AddCardToHand<Boost>()); // flush BronzeCore's opening batch
        await RelicCmd.Obtain<PlatinumCore>(ctx.Player);
        await PowerCmd.Apply<ReboundPower>(choiceCtx, ctx.Player.Creature, 1, ctx.Player.Creature, null);
        var beforeCount = ctx.Player.EncodePile.Count;

        var strike = await ctx.AddCardToHand<StrikeAutomaton>();
        await ctx.PlayCard(strike, ctx.Combat.HittableEnemies.First());

        Assert.AreEqual(beforeCount + 1, ctx.Player.EncodePile.Count, "Exactly one token should be encoded.");
        Assert.AreEqual(1, ctx.Player.EncodePile.OfType<CoreStrike>().Count(),
            "The encoded card should be a fresh Throw token, not the played Strike.");
        Assert.IsTrue(strike.Pile?.Type == PileType.Exhaust, "The played Strike itself ends up Exhausted.");
        Assert.AreEqual(0, ctx.Player.Creature.GetInstancedPowerAmountSum<ReboundPower>(),
            "Rebound already redirected the Strike (and spent its charge) before Platinum Core Exhausted it.");
    }

    // A dupe never resolves to anywhere (ceases to exist like any other dupe - see
    // CardModel.GetResultLocationForCardPlay), and Platinum Core explicitly skips IsDupe cards so a
    // duped Strike/Defend can't mint a free Throw/Catch token.
    [CardTest(typeof(Automaton.AutomatonCode.Core.Automaton))]
    public async Task DupedStrikeWithPlatinumCoreDoesNotEncodeAToken(TestContext ctx)
    {
        var choiceCtx = new BlockingPlayerChoiceContext();
        await ctx.PlayCard(await ctx.AddCardToHand<Boost>());
        await RelicCmd.Obtain<PlatinumCore>(ctx.Player);
        var beforeCount = ctx.Player.EncodePile.Count;

        var dupe = (await ctx.AddCardToHand<StrikeAutomaton>()).CreateDupeCompat();
        await CardCmd.AutoPlay(choiceCtx, dupe, ctx.Combat.HittableEnemies.First());

        Assert.AreEqual(beforeCount, ctx.Player.EncodePile.Count,
            "A duped Strike with Platinum Core should not mint an Throw token.");
    }

    // Characterization: a self-encodable card that Platinum Core does not claim is encoded once,
    // and a Rebound charge is kept, with Platinum Core present.
    [CardTest(typeof(Automaton.AutomatonCode.Core.Automaton))]
    public async Task EncodableCardIsEncodedOnceWithPlatinumCore(TestContext ctx)
    {
        var choiceCtx = new BlockingPlayerChoiceContext();
        await ctx.PlayCard(await ctx.AddCardToHand<Boost>());
        await RelicCmd.Obtain<PlatinumCore>(ctx.Player);
        await PowerCmd.Apply<ReboundPower>(choiceCtx, ctx.Player.Creature, 1, ctx.Player.Creature, null);
        var beforeCount = ctx.Player.EncodePile.Count;

        var card = await ctx.AddCardToHand<Boost>();
        await ctx.PlayCard(card);

        Assert.AreEqual(beforeCount + 1, ctx.Player.EncodePile.Count, "Boost should be encoded exactly once.");
        Assert.AreEqual(1, ctx.Player.Creature.GetInstancedPowerAmountSum<ReboundPower>(), "Rebound charge should be kept.");
    }

    // Regression guard: Bronze Orb redirects a played card's result location straight to
    // StashPile.Stash itself, bypassing StashCmd.Run (the "one and only stash flow" every other
    // stash entry point goes through). It used to insert at CardPilePosition.Top (front/foreground,
    // in front of whatever's already stashed) instead of Bottom (back/background, queued after
    // what's already there) like every other stash source.
    [CardTest(typeof(Automaton.AutomatonCode.Core.Automaton))]
    public async Task BronzeOrbStashesToTheBackOfThePile(TestContext ctx)
    {
        var choiceCtx = new BlockingPlayerChoiceContext();
        await ctx.ClearHand();
        await PowerCmd.Apply<BronzeOrbPower>(choiceCtx, ctx.Player.Creature, 1, ctx.Player.Creature, null);

        var filler = await ctx.AddCardToHand<Error>();
        await StashCmd.Stash(choiceCtx, filler);
        Assert.AreEqual(1, ctx.Player.StashPile.Count, "Setup: filler should be alone in the stash.");

        var target = ctx.Combat.HittableEnemies.First();
        var card = await ctx.AddCardToHand<Fortify>();
        await ctx.PlayCard(card, target);

        var stash = ctx.Player.StashPile;
        Assert.AreEqual(2, stash.Count, "Bronze Orb should have stashed the played card alongside the filler.");
        Assert.IsTrue(stash[0] == filler,
            "The filler stashed before Bronze Orb fired should stay at the front of the pile.");
        Assert.IsTrue(stash[1] == card,
            "Bronze Orb should stash to the back of the pile (CardPilePosition.Bottom), not the front.");
    }

    // Regression guard: Compilable.GetDescription used to copy only a plain scalar
    // into a fresh throwaway FunctionDynamicVar, discarding the source var's upgrade/highlight state
    // (DynamicVar.WasJustUpgraded). {CompileStrength:diff()} in encode.json colors the number based
    // on that state, so toggling a card's Normal/UG preview in the Library never changed the
    // "Compile" line's color even though the on-card text (which reuses the card's real DynamicVars
    // directly) colored correctly.
    [CardTest(typeof(Automaton.AutomatonCode.Core.Automaton))]
    public async Task CompileStrengthColorsWhenCardIsUpgraded(TestContext ctx)
    {
        var card = await ctx.AddCardToHand<Boost>();

        var beforeUpgrade = new StrengthCompile().GetDescription(card, false).GetFormattedText();
        Assert.IsTrue(!beforeUpgrade.Contains("[green]"),
            "A non-upgraded card's Compile line should not be colored.");

        card.UpgradeInternal();
        var afterUpgrade = new StrengthCompile().GetDescription(card, false).GetFormattedText();
        Assert.IsTrue(afterUpgrade.Contains("[green]"),
            "Compile's Strength value should be colored green when viewing the card's upgraded (UG) version.");
    }

    // Guard for Compilable's scalar derivation: Strength/Thorns take their merged value from the
    // source card's DynamicVar (Compilable.SourceVar), Error To Stash supplies a fixed 1.
    [CardTest(typeof(Automaton.AutomatonCode.Core.Automaton))]
    public async Task CompileValuesMergeOntoFunction(TestContext ctx)
    {
        var choiceCtx = new BlockingPlayerChoiceContext();
        Assert.AreEqual(3, AutomatonCmd.GetMax(ctx.Player), "Setup: test assumes an Encode pile of 3.");

        // BronzeCore may have left cards in the Encode pile; flush them so the next three form one Function.
        while (ctx.Player.EncodePile.Count > 0)
            await AutomatonCmd.EncodeCard<OilSpill>(ctx.Player, choiceCtx);

        await AutomatonCmd.EncodeCard<Boost>(ctx.Player, choiceCtx);
        await AutomatonCmd.EncodeCard<Spike>(ctx.Player, choiceCtx);
        await AutomatonCmd.EncodeCard<OilSpill>(ctx.Player, choiceCtx);

        var function = ctx.Player.Hand.OfType<FunctionCard>()
            .FirstOrDefault(f => f.SourceCards.Any(c => c is Spike));
        Assert.IsTrue(function != null, "Boost + Spike + Oil Spill should have compiled into a Function in hand.");

        var boost = await ctx.AddCardToHand<Boost>();
        var spike = await ctx.AddCardToHand<Spike>();
        Assert.AreEqual(boost.DynamicVars.Power<StrengthPower>().BaseValue,
            function!.DynamicVars["CompileStrength"].BaseValue,
            "Compile Strength should equal the source card's Strength var.");
        Assert.AreEqual(spike.DynamicVars.Power<ThornsPower>().BaseValue,
            function.DynamicVars["CompileThorns"].BaseValue,
            "Compile Thorns should equal the source card's Thorns var.");
        Assert.AreEqual(1m, function.DynamicVars["CompileErrors"].BaseValue,
            "Error To Stash should contribute its fixed 1.");
    }

    // Regression guard: the game runs Enchantment.OnPlay (Momentum's damage increment) after
    // CardModel.OnPlay, but Encode used to compile the Function inside the card's OnPlay wrapper. A
    // Momentum-enchanted card played as the last Encode slot therefore compiled with its stale,
    // pre-increment damage, and mid-sequence the Encode pile preview lagged one card behind.
    [CardTest(typeof(Automaton.AutomatonCode.Core.Automaton))]
    public async Task MomentumBonusIsIncludedWhenEncodedCardCompilesLast(TestContext ctx)
    {
        // BronzeCore auto-encodes a Defend + Strike on turn 1; flush that batch first.
        await ctx.PlayCard(await ctx.AddCardToHand<Boost>());

        await ctx.PlayCard(await ctx.AddCardToHand<Boost>());
        await ctx.PlayCard(await ctx.AddCardToHand<Boost>());

        var target = ctx.Combat.HittableEnemies.First();
        var oilSpill = await ctx.AddCardToHand<OilSpill>();
        CardCmd.Enchant<Momentum>(oilSpill, 3);
        var baseDamage = oilSpill.DynamicVars.Damage.BaseValue;
        await ctx.PlayCard(oilSpill, target);

        var function = ctx.Player.Hand.OfType<FunctionCard>().FirstOrDefault(f => f.SourceCards.Contains(oilSpill));
        Assert.IsTrue(function != null, "Boost x2 + Oil Spill should have compiled into a Function in hand.");
        Assert.AreEqual(baseDamage + 3, function!.DynamicVars.Damage.BaseValue,
            "Momentum's extra damage from the play that completed the Function must be part of the compiled Function.");
    }
    
    [CardTest(typeof(Automaton.AutomatonCode.Core.Automaton))]
    public async Task EncodeOrbTooltipReflectsElectromagneticCoil(TestContext ctx)
    {
        var withoutCoil = new LocString("static_hover_tips", "AUTOMATON-ENCODE_PILE.description");
        withoutCoil.Add("Max", AutomatonCmd.GetMax(ctx.Player));
        Assert.IsTrue(withoutCoil.GetFormattedText().Contains("3"),
            "Without Electromagnetic Coil, the Encode Orb tooltip should still say 3.");

        await RelicCmd.Obtain<ElectromagneticCoil>(ctx.Player);

        var withCoil = new LocString("static_hover_tips", "AUTOMATON-ENCODE_PILE.description");
        withCoil.Add("Max", AutomatonCmd.GetMax(ctx.Player));
        var text = withCoil.GetFormattedText();
        Assert.IsTrue(text.Contains("4"),
            "With Electromagnetic Coil, the Encode Orb tooltip should say 4, not the hardcoded 3.");
        Assert.IsTrue(!text.Contains("3"),
            "With Electromagnetic Coil, the Encode Orb tooltip should no longer mention 3.");
    }

    // Encode and Compile are real CardKeywords: an encodable card carries Encode, a card with a compile
    // effect carries Compile, and Strike/Defend (only encodable when a relic forces it) carry neither.
    [CardTest(typeof(Automaton.AutomatonCode.Core.Automaton))]
    public async Task EncodeAndCompileAreCardKeywords(TestContext ctx)
    {
        var boost = await ctx.AddCardToHand<Boost>();
        Assert.IsTrue(boost.Keywords.Contains(AutomatonKeyword.Encode), "Boost is encodable, so it has Encode.");
        Assert.IsTrue(boost.Keywords.Contains(AutomatonKeyword.Compile), "Boost has a compile effect, so it has Compile.");

        var frontload = await ctx.AddCardToHand<Frontload>();
        Assert.IsTrue(frontload.Keywords.Contains(AutomatonKeyword.Encode), "Frontload is encodable.");
        Assert.IsTrue(frontload.Keywords.Contains(AutomatonKeyword.Compile), "Frontload's Retain is a compile effect.");

        var strike = await ctx.AddCardToHand<StrikeAutomaton>();
        Assert.IsTrue(!strike.Keywords.Contains(AutomatonKeyword.Encode),
            "Strike is not encodable on play, so it must not have Encode.");

        Assert.AreEqual("Encode", new LocString("card_keywords", "AUTOMATON-ENCODE.title").GetFormattedText(),
            "Encode keyword title loc.");
        Assert.AreEqual("Compile", new LocString("card_keywords", "AUTOMATON-COMPILE.title").GetFormattedText(),
            "Compile keyword title loc.");
    }
}
