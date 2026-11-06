using Downfall.DownfallCode.Abstract;
using Guardian.GuardianCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
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
        // Normally the card is still sitting invisibly in the Draw pile here, so the move into
        // Stasis can skip visuals. But another turn-1 effect (e.g. Jeweled Mask) may have already
        // moved it into a visible pile (Hand) earlier in this same BeforeHandDraw phase — if we
        // skip visuals then, the removal from Hand never fires CardRemoved/ContentsChanged, and
        // the card's old on-screen node is orphaned in Hand instead of being cleaned up.
        var alreadyVisible = Card.Pile?.Type == PileType.Hand;
        await GuardianCmd.PutIntoStasis(Card, ctx, this, !alreadyVisible);
    }
}