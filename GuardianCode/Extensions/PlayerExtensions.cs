using BaseLib.Patches.Content;
using Guardian.GuardianCode.Piles;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace Guardian.GuardianCode.Extensions;

public static class PlayerExtensions
{
    extension(Player player)
    {
        public IReadOnlyList<CardModel> StasisPile =>
            CustomPiles.GetCustomPile(player.PlayerCombatState, GuardianPile.Stasis)?.Cards
            ?? [];
    }
}