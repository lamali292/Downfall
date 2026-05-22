using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Extensions;

public static class PlayerExtensions
{
    public static IReadOnlyList<CardModel> GetHand(this Player player)
    {
        return PileType.Hand.GetPile(player).Cards;
    }

    public static IReadOnlyList<CardModel> GetDiscard(this Player player)
    {
        return PileType.Discard.GetPile(player).Cards;
    }

    public static IReadOnlyList<CardModel> GetDraw(this Player player)
    {
        return PileType.Draw.GetPile(player).Cards;
    }

    public static IReadOnlyList<CardModel> GetDeck(this Player player)
    {
        return PileType.Deck.GetPile(player).Cards;
    }

    public static IReadOnlyList<CardModel> GetExhaust(this Player player)
    {
        return PileType.Exhaust.GetPile(player).Cards;
    }

    public static IEnumerable<CardModel> GetAllCards(this Player player)
    {
        return player.PlayerCombatState?.AllCards ?? [];
    }
}