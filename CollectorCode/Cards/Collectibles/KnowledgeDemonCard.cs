using Collector.CollectorCode.Cards.Token;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
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
        var pool = Owner.UnlockState.CharacterCardPools
            .Where( e => e != Owner.Character.CardPool)
            .SelectMany(cardPoolModel => cardPoolModel
                .AllCards
                .Where(c => c
                                .EnergyCost.Canonical >= 0
                            && !c.HasStarCostX &&  
                            c is { CanonicalStarCost: -1, CanBeGeneratedInCombat: true })
            );
        // we can't do ANY pool. 
        // it might be funny. but it will certainly break with other mods.
        // I don't want every buggy jank card to be draftable that's hidden in a random modded non-character pool. 
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
