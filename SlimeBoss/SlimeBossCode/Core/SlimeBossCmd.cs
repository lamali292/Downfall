using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SlimeBoss.SlimeBossCode.CustomEnums;

namespace SlimeBoss.SlimeBossCode.Core;

public static class SlimeBossCmd
{
    public static async Task Command(PlayerChoiceContext ctx, CardModel card)
    {
        throw new NotImplementedException();
    }

    public static async Task Slurp(CardModel card)
    {
        var amount = card.DynamicVars["Slurp"].IntValue;
        var licks = card.Owner.PlayerCombatState?.ExhaustPile.Cards
            .Where(e => e.Tags.Contains(SlimeBossTag.Lick))
            .ToList() ?? [];
    
        var unburied = licks.Where(e => !e.Keywords.Contains(SlimeBossKeyword.Buried)).ToList();
        var buried = licks.Where(e => e.Keywords.Contains(SlimeBossKeyword.Buried)).ToList();
    
        var cards = unburied
            .TakeRandom(Math.Min(amount, unburied.Count), card.Owner.RunState.Rng.CombatCardSelection)
            .ToList();
    
        if (cards.Count < amount)
            cards.AddRange(buried.TakeRandom(amount - cards.Count, card.Owner.RunState.Rng.CombatCardSelection));
    
        await CardPileCmd.Add(cards, PileType.Hand);
    }
}