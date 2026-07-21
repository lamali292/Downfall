using BaseLib.Patches.Content;
using Guardian.GuardianCode.Piles;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace Guardian.GuardianCode.Extensions;

public static class PlayerExtensions
{
    public static IReadOnlyList<CardModel> GetStasis(this Player player)
    {
        return CustomPiles.GetCustomPile(player.PlayerCombatState, GuardianPile.Stasis)?.Cards
               ?? [];
    }
}