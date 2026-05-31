using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Piles;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Cards.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class GreenpyreLocus : CollectorCardModel
{
    public GreenpyreLocus() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithCards(2, 1);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (Owner.Creature.CombatState == null) return;
        var rng = Owner.RunState.Rng.CombatCardGeneration;

        // Build 3 distinct weighted-rarity attack cards from ALL pools
        var cardsByRarity = ModelDb.CardPool<CollectibleCardPool>()
            .AllCards
            .GroupBy(c => c.Rarity)
            .ToDictionary(g => g.Key, g => g.ToList());

        var weightedAttacks = new List<CardModel>();
        var seen = new HashSet<string>();

        while (weightedAttacks.Count < 3)
        {
            var roll = rng.NextInt(100);
            var rarity = roll < 65 ? CardRarity.Common
                : roll < 90 ? CardRarity.Uncommon
                : CardRarity.Rare;

            if (!cardsByRarity.TryGetValue(rarity, out var pool)) continue;

            var candidate = rng.NextItem(pool);
            if (candidate == null || !seen.Add(candidate.Id.Entry)) continue;

            weightedAttacks.Add(Owner.Creature.CombatState.CreateCard(candidate, Owner));
        }

        // Let player choose 1 of 3
        var chosenCard = await CardSelectCmd.FromChooseACardScreen(
            ctx,
            weightedAttacks,
            Owner,
            true
        );

        if (chosenCard == null) return;

        await CardPileCmd.AddGeneratedCardToCombat(
            chosenCard,
            PileType.Hand,
            Owner
        );

        for (var i = 0; i < DynamicVars.Cards.IntValue; i++)
        {
            var copy = chosenCard.CreateClone();
            var result = await CardPileCmd.Add(copy, CollectorPile.Collected, CardPilePosition.Random);
            CardCmd.PreviewCardPileAdd(result, 0.2f);
        }
    }
}