using Downfall.DownfallCode.Interfaces;
using Guardian.GuardianCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Guardian.GuardianCode.Powers;

public class StasisEnginePower : GuardianPowerModel, IHasSecondAmount
{
    private int _triggers;


    public string GetSecondAmount()
    {
        return $"{_triggers}/3";
    }

    public override async Task AfterCardPlayedLate(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (_triggers >= 3 || cardPlay.Card.Owner != Owner.Player || cardPlay.Card.EnergyCost.GetResolved() != 0) return;
        _triggers++;
        InvokeDisplayAmountChanged();
        if (_triggers >= 3)
        {
            await PlayerCmd.GainEnergy(Amount, Owner.Player);
            await CardPileCmd.Draw(choiceContext, Amount, Owner.Player);
        }
    }

    public override Task BeforeHandDraw(Player player, PlayerChoiceContext ctx, ICombatState combatState)
    {
        if (player.Creature != Owner) return Task.CompletedTask;
        _triggers = 0;
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }
}