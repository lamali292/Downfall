using BaseLib.Cards;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Core;
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

    private readonly List<CardPoolModel> _validPools =
    [
        ModelDb.CardPool<ColorlessCardPool>(), ModelDb.CardPool<CurseCardPool>(), ModelDb.CardPool<StatusCardPool>(),
        ModelDb.CardPool<CollectibleCardPool>()
    ];
    

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var prismatic = Owner.UnlockState.CharacterCardPools.ToList();
        IEnumerable<CardModel>? pool = null;

        foreach (var cardPoolModel in prismatic)
        {
            var newCm = cardPoolModel.AllCards
                .Where(c => c.EnergyCost.Canonical >= 0 && !c.HasStarCostX &&
                            c is { CanonicalStarCost: -1, CanBeGeneratedInCombat: true }).Where(c =>
                    !(c.Keywords.Contains(CardKeyword.Unplayable) || c.Keywords.Contains(BaseLibKeywords.Purge) ||
                      c.Keywords.Contains(CardKeyword.Eternal)));
            //Must be playable, cannot cost stars, cannot be fleeting/purge and cannot be eternal.
            pool = pool is null ? newCm : pool.Concat(newCm);
        }

        var notPrismatic = ModelDb.AllSharedCardPools.ToList();
        foreach (var cardPoolModel in notPrismatic)
        {
            if (!_validPools.Contains(cardPoolModel))
            {
                continue;
                //Don't get from any non-allowed pools. (I.E event and token)
                //Only collect from safe pools (I.E Colourless, status, curse, collectibles...)
            }

            var newCm = cardPoolModel.AllCards
                .Where(c => c.EnergyCost.Canonical >= 0 && !c.HasStarCostX &&
                            c is { CanonicalStarCost: -1, CanBeGeneratedInCombat: true }).Where(c =>
                    !(c.Keywords.Contains(CardKeyword.Unplayable) || c.Keywords.Contains(BaseLibKeywords.Purge) ||
                      c.Keywords.Contains(CardKeyword.Eternal)));
            //Must be playable, cannot cost stars, cannot be fleeting/purge and cannot be eternal.
            pool = pool is null ? newCm : pool.Concat(newCm);
        }

        if (pool is null)
        {
            throw new NullReferenceException("No pool found");
        }

        var mungus = ModelDb.CardPool<EventCardPool>().AllCards
            .Where(c => c.Rarity == CardRarity.Ancient && c.EnergyCost.Canonical >= 0 && !c.HasStarCostX &&
                        c is { CanonicalStarCost: -1, CanBeGeneratedInCombat: true }).Where(c =>
                !(c.Keywords.Contains(CardKeyword.Unplayable) || c.Keywords.Contains(BaseLibKeywords.Purge)));
        //Event pool has unique restrictions to exclude any non-ancient cards due to the presence of things like "MadScience" which can cause issues when generated mid-combat.
        pool = pool.Concat(
            mungus);
        

        /*
        var pool = Owner.UnlockState.CharacterCardPools
            .Where( e => e != Owner.Character.CardPool)
            .SelectMany(cardPoolModel => cardPoolModel
                .AllCards
                .Where(c => c
                                .EnergyCost.Canonical >= 0
                            && !c.HasStarCostX &&  
                            c is { CanonicalStarCost: -1, CanBeGeneratedInCombat: true })
            );
		*/
        // we can't do ANY pool. 
        // it might be funny. but it will certainly break with other mods.
        // I don't want every buggy jank card to be draftable that's hidden in a random modded non-character pool. 

        // There are enough safeguards already, the card must be playable and cant cost stars, 
        // I added additional checks for "Purge" (Fleeting), "Unplayable" if for some reason the modder did not set invalid cost and "Eternal" for cards that shouldn't be in the random pool anyway.
		
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
