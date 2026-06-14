using Downfall.DownfallCode.Abstract;
using Guardian.GuardianCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Guardian.GuardianCode.Enchantments;

public class Temporal : DownfallEnchantmentModel<Core.Guardian>
{
    public override async Task BeforeHandDrawLate(
        Player player,
        PlayerChoiceContext ctx,
        ICombatState combatState)
    {
        if (player != Card.Owner || player.PlayerCombatState is not { TurnNumber: 1 }) return;
        await GuardianCmd.PutIntoStasis(Card, ctx, this, true);
    }
}