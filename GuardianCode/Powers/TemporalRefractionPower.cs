using BaseLib.Abstracts;
using Downfall.DownfallCode.Interfaces;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.Events;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Guardian.GuardianCode.Powers;

public class TemporalRefractionPower : GuardianPowerModel, IModifyGemEffect, IHasSecondAmount, IAfterGemPlayed
{
    private int UsedAmount { get; set; }

    public Task AfterGemPlayed(PlayerChoiceContext ctx, GemModel gemModel, CardPlay? cardPlay)
    {
        if (Owner != gemModel.Card.Owner.Creature || UsedAmount >= Amount) return Task.CompletedTask;
        UsedAmount++;
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }

    public string GetSecondAmount()
    {
        return $"{UsedAmount}";
    }

    public decimal ModifyGemEffect(GemModel model, decimal baseValue, CardModel card)
    {
        return Owner == card.Owner.Creature && UsedAmount < Amount && model.SocketIndex < Amount
            ? baseValue * 2
            : baseValue;
    }
}