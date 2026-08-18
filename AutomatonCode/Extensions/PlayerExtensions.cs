using Automaton.AutomatonCode.Piles;
using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Extensions;

public static class PlayerExtensions
{
    extension(Player player)
    {
        public IReadOnlyList<CardModel> StashPile =>
            CustomPiles.GetCustomPile(player.PlayerCombatState, StashPile.Stash)?.Cards
            ?? [];

        public IReadOnlyList<CardModel> EncodePile =>
            CustomPiles.GetCustomPile(player.PlayerCombatState, EncodePile.FunctionSequence)?.Cards
            ?? [];
    }
}