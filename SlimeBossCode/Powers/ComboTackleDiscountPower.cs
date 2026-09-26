using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Powers;

/// <summary>
/// One stack = one future card discounted by 1 [E]. Mirrors the base game's FreeSkillPower pattern
/// (TryModifyEnergyCostInCombat + Decrement on BeforeCardPlayed) instead of a bespoke remove-after-play.
/// </summary>
public class ComboTackleDiscountPower : SlimeBossPowerModel
{
    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        if (card.Owner.Creature != Owner || Amount <= 0) return false;

        modifiedCost = Math.Max(0, originalCost - 1);
        return true;
    }

    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature == Owner) await PowerCmd.Decrement(this);
    }
}
