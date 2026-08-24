using Awakened.AwakenedCode.Piles;
using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace Awakened.AwakenedCode.Extensions;

public static class PlayerExtensions
{
    extension(Player player)
    {
        public IReadOnlyList<CardModel> Spellbook =>
            CustomPiles.GetCustomPile(player.PlayerCombatState, AwakenedPile.Spellbook)?.Cards
            ?? [];
    }
}