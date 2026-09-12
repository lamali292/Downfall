using Collector.CollectorCode.Cards.Token;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Encounters;
namespace Collector.CollectorCode.Cards.Collectibles;

public class KnowledgeDemonCard : Collectible<KnowledgeDemonBoss>
{
    public KnowledgeDemonCard() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self, 0.1f)
    {
        WithCards(5);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {

        var prismatic = Owner.UnlockState.CharacterCardPools.ToList();
        IEnumerable<CardModel>? pool = null;
        
        foreach (var cardPoolModel in prismatic)
        {
            var newCm = cardPoolModel.AllCards.Where(c => c.EnergyCost.Canonical >= 0 && c is { CanonicalStarCost: -1, CanBeGeneratedInCombat: true});//Not unplayable and does not have star cost.
            pool = pool is null ? newCm : pool.Concat(newCm);
        }
        var notPrismatic = ModelDb.AllSharedCardPools.ToList();
        foreach (var cardPoolModel in notPrismatic)
        {
            if (cardPoolModel is DeprecatedCardPool)
            {
                continue;//Dont get any deprecated cards.
            }
            var newCm = cardPoolModel.AllCards.Where(c => c.EnergyCost.Canonical >= 0 && c is { CanonicalStarCost: -1, CanBeGeneratedInCombat: true});//Not unplayable and does not have star cost.
            pool = pool is null ? newCm : pool.Concat(newCm);
        }
        
        if (pool is null)
        {
            throw new NullReferenceException("No pool found");
        }
        
        var mungus = ModelDb.CardPool<EventCardPool>().AllCards.Where(c => c.EnergyCost.Canonical >= 0 && c is { CanonicalStarCost: -1, CanBeGeneratedInCombat: true});
        pool = pool.Concat(mungus);//The event card pool is removed at some point from ALlSharedPools for some reason so i add it back in here.
        
        var list = CardFactory.GetDistinctForCombat(Owner, pool, 
            DynamicVars.Cards.IntValue, Owner.RunState.Rng.CombatCardGeneration).ToList();
        foreach (var card in list)
            CardCmd.Upgrade(card);
        var card1 = await CardSelectCmd.FromChooseACardScreen(ctx, list, Owner);
        if (card1 == null) return;
        if (IsUpgraded)
        {
            card1.SetToFreeThisTurn();
        }
        else
        {
            card1.EnergyCost.AddThisTurnOrUntilPlayed(-1);
        }
        await CardPileCmd.AddGeneratedCardToCombat(card1, PileType.Hand, Owner);
    }
}
