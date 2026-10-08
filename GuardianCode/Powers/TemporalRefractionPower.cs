using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.Events;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Guardian.GuardianCode.Powers;

public class TemporalRefractionPower : GuardianPowerModel, IModifyGemEffect, IAfterGemPlayed
{
    private int UsedAmount { get; set; }

    public Task AfterGemPlayed(PlayerChoiceContext ctx, GemModel gemModel, CardPlay? cardPlay)
    {
        if (Owner != gemModel.Card?.Owner.Creature || UsedAmount >= Amount) return Task.CompletedTask;
        UsedAmount++;
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }

    public override int DisplayAmount => Amount - UsedAmount;

    public decimal ModifyGemEffect(GemModel model, decimal baseValue, CardModel? card)
    {
        // Gems on the same card activate in socket order, each consuming one stack in turn, so a
        // gem at SocketIndex s only gets doubled if the s gems before it on this card (which would
        // consume first) still leave a stack for it - i.e. UsedAmount + s < Amount. This also keeps
        // card-preview text in sync with what actually happens when the card is played: previewing
        // every gem against the same not-yet-incremented UsedAmount (the old `SocketIndex < Amount`
        // check) could show two gems on one card both doubling when only the first one really would.
        return Owner == card?.Owner.Creature && UsedAmount + model.SocketIndex < Amount
            ? baseValue * 2
            : baseValue;
    }

    public override Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        UsedAmount = 0;
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }
}