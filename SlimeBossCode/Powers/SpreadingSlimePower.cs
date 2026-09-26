using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Powers;

/// <summary>
/// The first Amount Status cards played each turn are free. Mirrors vanilla FreeSkillPower's shape
/// (TryModifyEnergyCostInCombat + BeforeCardPlayed) but resets a per-turn counter instead of consuming a stack.
/// </summary>
public class SpreadingSlimePower : SlimeBossPowerModel
{
    private int RemainingThisTurn
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            InvokeDisplayAmountChanged();
        }
    }

    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        if (card.Owner.Creature != Owner || card.Type != CardType.Status || RemainingThisTurn <= 0) 
            return false;
            
        modifiedCost = 0;
        return true;
    }

    public override int DisplayAmount => RemainingThisTurn;

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature == Owner && cardPlay.Card.Type == CardType.Status && RemainingThisTurn > 0)
        {
            RemainingThisTurn--;
        }
        return Task.CompletedTask;
    }

    public override Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (participants.Contains(Owner))
        {
            RemainingThisTurn = Amount;
        }
        return Task.CompletedTask;
    }
}