using Automaton.AutomatonCode.Cards.Rare;
using Champ.ChampCode.Cards.Ancient;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Encounters;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.TestCode;

public class TestRuns
{
    [CardTest]
    public async Task ExecutionNormalTargetDealsBaseDamage(TestContext ctx)
    {
        var target = ctx.Combat.HittableEnemies.First();
        var startingHp = target.CurrentHp;
        var card = await ctx.AddCardToHand<Execution>();
        await ctx.PlayCard(card, target);
        Assert.IsTrue(target.CurrentHp < startingHp, "Target should have taken damage.");
    }

    [CardTest(typeof(Champ.ChampCode.Core.Champ), typeof(SlimesNormal))]
    public async Task ExecutionLowHealthTargetRefundsEnergy(TestContext ctx)
    {
        await ctx.AddCardToTopOfDraw<ProtoShield>();
        var card = await CardPileCmd.Draw(new BlockingPlayerChoiceContext(), ctx.Player);
        Assert.IsTrue(card is ProtoShield);
        Assert.AreEqual(3, ctx.Player.Creature.GetPowerAmount<PlatingPower>(), "Energy should be refunded on kill/low HP.");
    }
    
    [CardTest(typeof(Automaton.AutomatonCode.Core.Automaton))]
    public async Task PlayAutomatonCards(TestContext ctx) => await PlayAllCard(ctx);
    
    [CardTest(typeof(Awakened.AwakenedCode.Core.Awakened))]
    public async Task PlayAwakenedCards(TestContext ctx) => await PlayAllCard(ctx);
    
    [CardTest(typeof(Champ.ChampCode.Core.Champ))]
    public async Task PlayChampCards(TestContext ctx) => await PlayAllCard(ctx);
    
    [CardTest(typeof(Guardian.GuardianCode.Core.Guardian))]
    public async Task PlayGuardianCards(TestContext ctx) => await PlayAllCard(ctx);
    
    [CardTest(typeof(Hermit.HermitCode.Core.Hermit))]
    public async Task PlayHermitCards(TestContext ctx) => await PlayAllCard(ctx);
    
    [CardTest(typeof(Hexaghost.HexaghostCode.Core.Hexaghost))]
    public async Task PlayHexaghostCards(TestContext ctx) => await PlayAllCard(ctx);

    [CardTest(typeof(SlimeBoss.SlimeBossCode.Core.SlimeBoss))]
    public async Task PlaySlimeBossCards(TestContext ctx) => await PlayAllCard(ctx);
    
    [CardTest(typeof(Snecko.SneckoCode.Core.Snecko))]
    public async Task PlaySneckoCards(TestContext ctx) => await PlayAllCard(ctx);
    
    private async Task PlayAllCard(TestContext ctx)
    {
        var champPool = ctx.Player.Character.CardPool;
        var relicPool = ctx.Player.Character.RelicPool;
        var relicsToTest = relicPool.AllRelics.ToList();
        
        foreach (var relicModel in relicsToTest)
        {
            await RelicCmd.Obtain(relicModel.ToMutable(), ctx.Player);
        }
        
        var cardsToTest = champPool.AllCards.ToList();
        var failedCards = new List<(string CardName, Exception Exception)>();
        foreach (var cardModel in cardsToTest)
        {
            try
            {
                var card = ctx.Combat.CreateCard(cardModel, ctx.Player);
                await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, ctx.Player);
                var target = card.TargetType switch
                {
                    TargetType.AnyEnemy => ctx.Combat.HittableEnemies.FirstOrDefault(),
                    _ => null
                };
                target?.MaxHp = 999;
                target?.CurrentHp = 999;
            
                
                // play card
                TestMainFile.Logger.Info($"[{card.Title}] played");
                await ctx.PlayCard(card, target);
                
                // draw card
                TestMainFile.Logger.Info($"[{card.Title}] drawed");
                var card2 = ctx.Combat.CreateCard(cardModel, ctx.Player);
                await CardPileCmd.AddGeneratedCardToCombat(card2, PileType.Draw, ctx.Player, CardPilePosition.Top);
                await CardPileCmd.Draw(new BlockingPlayerChoiceContext(), ctx.Player);
                
                // exhaust card
                TestMainFile.Logger.Info($"[{card.Title}] exhausted");
                var card3 = ctx.Combat.CreateCard(cardModel, ctx.Player);
                await CardPileCmd.AddGeneratedCardToCombat(card3, PileType.Hand, ctx.Player);
                await CardCmd.Exhaust(new BlockingPlayerChoiceContext(), card3);
                
                // discard card
                TestMainFile.Logger.Info($"[{card.Title}] discarded");
                var card4 = ctx.Combat.CreateCard(cardModel, ctx.Player);
                await CardPileCmd.AddGeneratedCardToCombat(card4, PileType.Hand, ctx.Player);
                await CardCmd.Discard(new BlockingPlayerChoiceContext(), card4);
                
                // play some other other cards
                var strike = ctx.Combat.CreateCard(ModelDb.Card<StrikeIronclad>(), ctx.Player);
                var defend = ctx.Combat.CreateCard(ModelDb.Card<DefendIronclad>(), ctx.Player);
                await CardPileCmd.AddGeneratedCardToCombat(strike, PileType.Hand, ctx.Player);
                await CardPileCmd.AddGeneratedCardToCombat(defend, PileType.Hand, ctx.Player);
                await ctx.PlayCard(strike, target);
                await ctx.PlayCard(defend, target);
                
                var card5 = ctx.Combat.CreateCard(cardModel, ctx.Player);
                await CardPileCmd.AddGeneratedCardToCombat(card5, PileType.Hand, ctx.Player);
                
                // next turn
                PlayerCmd.EndTurn(ctx.Player, false);
            }
            catch (Exception ex)
            {
                failedCards.Add((cardModel.GetType().Name, ex.InnerException ?? ex));
            }
        }
        
        if (failedCards.Count > 0)
        {
            var summary = string.Join("\n", failedCards.Select(f => $"  - {f.CardName}: {f.Exception.Message}"));
            Assert.IsTrue(false, $"{failedCards.Count} card(s) failed during playback:\n{summary}");
        }
    }
}