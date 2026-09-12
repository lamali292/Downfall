using BaseLib.Cards;
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
    
    private static bool IsCardWeWant(CardModel card)
    {
        return card.EnergyCost.Canonical >= 0 
               && card is { CanonicalStarCost: -1, CanBeGeneratedInCombat: true } 
               &&  !card.Keywords.Contains(CardKeyword.Unplayable)
               &&  !card.Keywords.Contains(BaseLibKeywords.Purge) 
               &&  !card.Keywords.Contains(CardKeyword.Eternal);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {

        var pool = Owner.UnlockState.CharacterCardPools
            .Append(ModelDb.CardPool<EventCardPool>())
            .Append(ModelDb.CardPool<ColorlessCardPool>())
            .Append(ModelDb.CardPool<CurseCardPool>())
            .Append(ModelDb.CardPool<QuestCardPool>())
            .Append(ModelDb.CardPool<StatusCardPool>())
            .Append(ModelDb.CardPool<TokenCardPool>())
            .SelectMany(cardPoolModel => cardPoolModel.AllCards.Where(IsCardWeWant));

        var list = CardFactory.GetDistinctForCombat(Owner, pool, 
            DynamicVars.Cards.IntValue, Owner.RunState.Rng.CombatCardGeneration).ToList();
        foreach (var card in list)
        {
            CardCmd.Upgrade(card);
            if (IsUpgraded)
            {
                card.SetToFreeThisTurn();
            }
            else
            {
                card.EnergyCost.AddThisTurnOrUntilPlayed(-1);
            }
        }
         
        var card1 = await CardSelectCmd.FromChooseACardScreen(ctx, list, Owner);
        if (card1 == null) return;
        await CardPileCmd.AddGeneratedCardToCombat(card1, PileType.Hand, Owner);
    }
}
