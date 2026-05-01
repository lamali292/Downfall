using Downfall.DownfallCode.Interfaces;
using Guardian.GuardianCode.Cards;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.Events;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Guardian.GuardianCode.Powers;

public class TemporalRefractionPower : GuardianPowerModel, IModifyGemEffect, IHasSecondAmount
{
    private int UsedAmount { get; set; }

    public string GetSecondAmount()
    {
        return $"{UsedAmount}";
    }

    public decimal ModifyGemEffect(GemModel model, decimal baseValue, CardModel card)
    {
        return Owner == card.Owner.Creature && UsedAmount < Amount ? baseValue * 2 : baseValue;
    }

    public override Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        UsedAmount = 0;
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (Owner != cardPlay.Card.Owner.Creature || UsedAmount >= Amount || cardPlay.Card is not GuardianCardModel
            {
                Gems.Count: > 0
            }) return Task.CompletedTask;
        UsedAmount++;
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }
}