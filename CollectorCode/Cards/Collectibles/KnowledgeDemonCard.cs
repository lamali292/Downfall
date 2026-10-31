using Downfall.DownfallCode.Extensions;
using BaseLib.Cards;
using Collector.CollectorCode.Cards.Rare;
using Collector.CollectorCode.Cards.Token;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
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
        if (//Initial checks (for all cards)
            card.EnergyCost.Canonical >= 0
            && card is { CanonicalStarCost: -1, CanBeGeneratedInCombat: true }
            && !card.Keywords.Contains(CardKeyword.Unplayable)
            && !card.Keywords.Contains(BaseLibKeywords.Purge)
            && !card.Keywords.Contains(CardKeyword.Eternal)
            && card.Rarity != CardRarity.Basic)
        {
            return card.Rarity switch
            {//Special handling for event and colorless.
                CardRarity.Event => PermitEvent(card),
                CardRarity.Ancient => PermitAncient(card),
                 _ => true
            };
        }
        return false;
    }

    private static readonly List<CardModel> PermittedEvent =
    [
        new ByrdSwoop(), new Exterminate(), new Peck(), new Rebound(), new RipAndTear(), new Squash(),
        new Enlightenment(), new FeedingFrenzy(), new Distraction(), new Outmaneuver(), new Stack(), new Entrench(),
        new Metamorphosis(), new ToricToughness(), new Caltrops(), new Caltrops(), new HelloWorld()
    ];
    
    private static readonly List<CardModel> PermittedAncient =
    [
        new Suppress(), new Break(), new Maul(), new NeowsFury(), new Whistle(), new StashAway(), new Wish(),
        new Abundance(), new Apparition(), new Apotheosis(),
        new Relax(), new Corruption(), new WraithForm()
    ];
    
    private static bool PermitEvent(CardModel card)
    {
        //To avoid unwanted instability, colourless cards work on an approved items only list.
        return PermittedEvent.Contains(card);
    }
    
    private static bool PermitAncient(CardModel card)
    {
        //Ancient card list is also manually filtered.
        return PermittedAncient.Contains(card);
    }
    
    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {

        var pool = Owner.UnlockState.CharacterCardPools
            .Append(ModelDb.CardPool<ColorlessCardPool>())
            .Append(ModelDb.CardPool<CurseCardPool>())
            .Append(ModelDb.CardPool<StatusCardPool>())
            .Append(ModelDb.CardPool<TokenCardPool>())
            .Append(ModelDb.CardPool<EventCardPool>())
            .SelectMany(cardPoolModel => cardPoolModel.AllCards.Where(IsCardWeWant));

        // CardFactory.GetDistinctForCombat drops Ancient and Event cards, which this card wants to roll, so it picks
        // by hand and only applies the single/multiplayer constraint the factory would.
        var multiplayer = Owner.RunState.Players.Count > 1;
        var forbidden = multiplayer ? CardMultiplayerConstraint.SingleplayerOnly : CardMultiplayerConstraint.MultiplayerOnly;
        var list = pool
            .Where(c => c.MultiplayerConstraint != forbidden)
            .Distinct()
            .TakeRandom(DynamicVars.Cards.IntValue, Owner.RunState.Rng.CombatCardGeneration)
            .Select(c => Owner.Creature.CombatState!.CreateCard(c, Owner))
            .ToList();
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
