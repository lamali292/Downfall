using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Commands;

public class DownfallCardCmd
{
    public static async Task Insert(CardModel card, Player player)
    {
        var copy = player.Creature.CombatState!.CreateCard(card, player);
        var result = await CardPileCmd.AddGeneratedCardToCombat(copy, PileType.Draw, true, CardPilePosition.Random);
        if (result.success)
            CardCmd.PreviewCardPileAdd(result);
    }

    public static async Task Insert(IEnumerable<CardModel> cards, Player player)
    {
        var copies = cards
            .Select(card => player.Creature.CombatState!.CreateCard(card, player))
            .ToList();

        var results = await CardPileCmd.AddGeneratedCardsToCombat(copies, PileType.Draw, true, CardPilePosition.Random);
        CardCmd.PreviewCardPileAdd(results);
    }

    public static async Task DiscardGenerated(CardModel card, Player player)
    {
        var dazed = player.Creature.CombatState!.CreateCard(card, player);
        var result = await CardPileCmd.AddGeneratedCardToCombat(dazed, PileType.Discard, true);
        if (result.success)
            CardCmd.PreviewCardPileAdd(result);
    }
}