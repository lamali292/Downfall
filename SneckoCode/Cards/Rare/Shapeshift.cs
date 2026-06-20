using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Cards.Rare;

[Pool(typeof(SneckoCardPool))]
public class Shapeshift  : SneckoCardModel
{
    public Shapeshift() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var rng = Owner.RunState.Rng.CombatCardGeneration;
        var allOffclass = SneckoModel.GetSneckoCards(Owner).ToList();
        var byRarity = allOffclass
            .GroupBy(c => c.Rarity)
            .ToDictionary(g => g.Key, g => g.ToList());

        var cards = Owner.GetHand().ToList();
        foreach (var card in cards)
        {
            if (!byRarity.TryGetValue(card.Rarity, out var choices) || choices.Count == 0) continue;
            var pick = choices.Where(c => c.Id != card.Id).ToList();   // exclude the same card
            if (pick.Count == 0) continue;
            var template = rng.NextItem(pick);
            if  (template == null) continue;
            var replacement = CombatState?.CreateCard(template, Owner);
            if (replacement == null) continue;

            await CardCmd.Transform(card, replacement);
            if (base.IsUpgraded)
            {
                CardCmd.Upgrade(replacement);
            }
        }
    }
}