using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Extensions;

public static class PlayerExtensions
{
    public static IReadOnlyList<CardModel> GetHand(this Player player, Func<CardModel, bool>? filter = null)
    {
        var cards = PileType.Hand.GetPile(player).Cards;
        return filter == null ? cards : cards.Where(filter).ToList();
    }

    public static IReadOnlyList<CardModel> GetDiscard(this Player player, Func<CardModel, bool>? filter = null)
    {
        var cards = PileType.Discard.GetPile(player).Cards;
        return filter == null ? cards : cards.Where(filter).ToList();
    }

    public static IReadOnlyList<CardModel> GetDraw(this Player player, Func<CardModel, bool>? filter = null)
    {
        var cards = PileType.Draw.GetPile(player).Cards;
        return filter == null ? cards : cards.Where(filter).ToList();
    }

    public static IReadOnlyList<CardModel> GetDeck(this Player player, Func<CardModel, bool>? filter = null)
    {
        var cards = PileType.Deck.GetPile(player).Cards;
        return filter == null ? cards : cards.Where(filter).ToList();
    }

    public static IReadOnlyList<CardModel> GetExhaust(this Player player, Func<CardModel, bool>? filter = null)
    {
        var cards = PileType.Exhaust.GetPile(player).Cards;
        return filter == null ? cards : cards.Where(filter).ToList();
    }

    public static IEnumerable<CardModel> GetAllCards(this Player player, Func<CardModel, bool>? filter = null)
    {
        var cards = player.PlayerCombatState?.AllCards ?? [];
        return filter == null ? cards : cards.Where(filter);
    }
}