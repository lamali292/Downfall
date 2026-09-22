using Automaton.AutomatonCode.Cards.Rare;
using Champ.ChampCode.Cards.Ancient;
using Downfall.DownfallCode.Compatibility;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Models.Encounters;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.TestCode;

public class TestRuns
{
    // ---- single-combat tests: return Task, take TestContext ----

    [CardTest]
    public async Task ExecutionNormalTargetDealsBaseDamage(TestContext ctx)
    {
        var target = ctx.Combat.HittableEnemies.First();
        var startingHp = target.CurrentHp;
        var card = await ctx.AddCardToHand<Execution>();
        await ctx.PlayCard(card, target);
        Assert.IsTrue(target.CurrentHp < startingHp, "Target should have taken damage.");
    }

    // ---- pool tests: return IEnumerable<CardTestCase>, take CharacterModel ----
    // NOTE: plain (non-async) generators — the runner drives each case in its own combat.

    [CardTest(typeof(Automaton.AutomatonCode.Core.Automaton))]
    public IEnumerable<CardTestCase> PlayAutomatonCards(CharacterModel character) => PlayAllCards(character);

    [CardTest(typeof(Awakened.AwakenedCode.Core.Awakened))]
    public IEnumerable<CardTestCase> PlayAwakenedCards(CharacterModel character) => PlayAllCards(character);

    [CardTest(typeof(Champ.ChampCode.Core.Champ))]
    public IEnumerable<CardTestCase> PlayChampCards(CharacterModel character) => PlayAllCards(character);

    [CardTest(typeof(Guardian.GuardianCode.Core.Guardian))]
    public IEnumerable<CardTestCase> PlayGuardianCards(CharacterModel character) => PlayAllCards(character);

    [CardTest(typeof(Hermit.HermitCode.Core.Hermit))]
    public IEnumerable<CardTestCase> PlayHermitCards(CharacterModel character) => PlayAllCards(character);

    [CardTest(typeof(Hexaghost.HexaghostCode.Core.Hexaghost))]
    public IEnumerable<CardTestCase> PlayHexaghostCards(CharacterModel character) => PlayAllCards(character);

    [CardTest(typeof(SlimeBoss.SlimeBossCode.Core.SlimeBoss))]
    public IEnumerable<CardTestCase> PlaySlimeBossCards(CharacterModel character) => PlayAllCards(character);

    [CardTest(typeof(Snecko.SneckoCode.Core.Snecko))]
    public IEnumerable<CardTestCase> PlaySneckoCards(CharacterModel character) => PlayAllCards(character);

    [CardTest(typeof(Collector.CollectorCode.Core.Collector))]
    public IEnumerable<CardTestCase> PlayCollectorCards(CharacterModel character) => PlayAllCards(character);

    
    private IEnumerable<CardTestCase> PlayAllCards(CharacterModel character)
    {
        return character.CardPool.AllCards.Select(model => new CardTestCase(model.GetType().Name, async ctx =>
        {
            TestMainFile.Logger.Info($"CardTestCase : {model.Title}");
            // combat is already fresh (runner called FreshCombat); relics already granted once.
            var card = ctx.Combat.CreateCard(model, ctx.Player);
            await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, ctx.Player);

            // Regression guard: a missing/misregistered loc table doesn't throw, it silently falls
            // back to the raw key (e.g. "cards.SLIMEBOSS-SERVE_PROTECT.description") - fetch both
            // once here so every card's loc actually resolves, not just whichever ones happen to
            // have a dedicated description test.
            var title = card.Title;
            var description = card.GetDescriptionForPile(PileType.Hand);
            Assert.IsTrue(!string.IsNullOrEmpty(title) && !title.Contains(card.Id.Entry),
                $"Title for {card.Id.Entry} looks unresolved: '{title}'.");
            Assert.IsTrue(!string.IsNullOrEmpty(description) && !description.Contains(card.Id.Entry),
                $"Description for {card.Id.Entry} looks unresolved: '{description}'.");

            var target = card.TargetType == TargetType.AnyEnemy
                ? ctx.Combat.HittableEnemies.FirstOrDefault()
                : null;
            if (target != null) { target.MaxHp = 9999; target.CurrentHp = 9999; }

            await ctx.PlayCard(card, target);

            PlayerCmd.EndTurn(ctx.Player, false);
            
            var card2 = ctx.Combat.CreateCard(model, ctx.Player);
            await CardPileCmd.AddGeneratedCardToCombat(card2, PileType.Draw, ctx.Player, CardPilePosition.Top);
            var a = await CardPileCmd.Draw(new BlockingPlayerChoiceContext(), ctx.Player);
            if (a != null) Assert.AreEqual(card2, a);

            PlayerCmd.EndTurn(ctx.Player, false);
            
            var card3 = ctx.Combat.CreateCard(model, ctx.Player);
            await CardPileCmd.AddGeneratedCardToCombat(card3, PileType.Hand, ctx.Player);
            var b = await CardCmdCompatibility.Exhaust(new BlockingPlayerChoiceContext(), card3);
            if (b.HasValue) Assert.AreEqual(card3, b.Value.cardAdded);

            PlayerCmd.EndTurn(ctx.Player, false);
            
            var card4 = ctx.Combat.CreateCard(model, ctx.Player);
            await CardPileCmd.AddGeneratedCardToCombat(card4, PileType.Hand, ctx.Player);
            await CardCmd.Discard(new BlockingPlayerChoiceContext(), card4);

            PlayerCmd.EndTurn(ctx.Player, false);
            
            var strike = ctx.Combat.CreateCard(ModelDb.Card<StrikeIronclad>(), ctx.Player);
            var defend = ctx.Combat.CreateCard(ModelDb.Card<DefendIronclad>(), ctx.Player);
            await CardPileCmd.AddGeneratedCardToCombat(strike, PileType.Hand, ctx.Player);
            await CardPileCmd.AddGeneratedCardToCombat(defend, PileType.Hand, ctx.Player);
            await ctx.PlayCard(strike, target);
            await ctx.PlayCard(defend, target);

            var card5 = ctx.Combat.CreateCard(model, ctx.Player);
            await CardPileCmd.AddGeneratedCardToCombat(card5, PileType.Hand, ctx.Player);
        }));
    }
}