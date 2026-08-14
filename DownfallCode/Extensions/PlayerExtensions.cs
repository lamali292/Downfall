using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Extensions;

public static class PlayerExtensions
{
    
    public static IReadOnlyList<Player> GetAllPlayers(this Player player)
    {
        return player.Creature.CombatState!.GetTeammatesOf(player.Creature)
            .Where(e => e.IsAlive)
            .Select(c => c.Player)
            .OfType<Player>()
            .ToArray();
    }

    public static IReadOnlyList<Player> GetOtherPlayers(this Player player)
    {
        return player.GetAllPlayers().Where(p => p != player).ToArray();;
    }

    public static Player? GetRandomOtherPlayer(this Player player)
    {
        return player.RunState.Rng.CombatTargets.NextItem(player.GetOtherPlayers());
    }
    
    
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