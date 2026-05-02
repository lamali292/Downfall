using Downfall.DownfallCode.Interfaces;
using Guardian.GuardianCode.Cards;
using Guardian.GuardianCode.Cards.Abstract;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.Events;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Guardian.GuardianCode.Powers;

public class TemporalRefractionPower : GuardianPowerModel, IModifyGemEffect, IHasSecondAmount, IAfterGemPlayed
{
    private int UsedAmount { get; set; }

    public string GetSecondAmount()
    {
        return $"{UsedAmount}";
    }

    public decimal ModifyGemEffect(GemModel model, decimal baseValue, CardModel card)
    {
        return Owner == card.Owner.Creature && UsedAmount < Amount && model.SocketIndex < Amount ? baseValue * 2 : baseValue;
    }
    
    public Task AfterGemPlayed(PlayerChoiceContext ctx, GemModel gemModel, CardPlay? cardPlay)
    {
        if (Owner != gemModel.Card.Owner.Creature || UsedAmount >= Amount) return Task.CompletedTask;
        UsedAmount++;
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }
}