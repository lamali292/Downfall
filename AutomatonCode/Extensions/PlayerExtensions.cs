using Automaton.AutomatonCode.Piles;
using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Extensions;

public static class PlayerExtensions
{
    public static IReadOnlyList<CardModel> GetStash(this Player player)
    {
        return CustomPiles.GetCustomPile(player.PlayerCombatState, StashPile.Stash)?.Cards
               ?? [];
    }

    public static IReadOnlyList<CardModel> GetEncode(this Player player)
    {
        return CustomPiles.GetCustomPile(player.PlayerCombatState, EncodePile.FunctionSequence)?.Cards
               ?? [];
    }
}