using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Powers;

/// <summary>One stack = one future Status card played free. Same shape as vanilla FreeSkillPower.</summary>
public class GlopChopPower : SlimeBossPowerModel
{
    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        if (card.Owner.Creature != Owner || card.Type != CardType.Status || Amount <= 0) return false;
        modifiedCost = 0;
        return true;
    }

    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature == Owner && cardPlay.Card.Type == CardType.Status)
            await PowerCmd.Decrement(this);
    }
}
