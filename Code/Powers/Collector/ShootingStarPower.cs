using Downfall.Code.Abstract;
using Downfall.Code.Events;
using Downfall.Code.Interfaces;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Powers.Collector;

public class ShootingStarPower : CollectorPowerModel, IOnPyre, IHasSecondAmount
{
    private int _usesThisTurn;

    public string GetSecondAmount()
    {
        return $"{Amount - _usesThisTurn}";
    }

    public async Task OnPyre(PlayerChoiceContext ctx, CardModel card, CardModel pyred)
    {
        if (card.Owner.Creature != Owner || pyred.Type != CardType.Attack || _usesThisTurn >= Amount) return;
        var copy = pyred.CreateClone();
        copy.EnergyCost.SetUntilPlayed(0);
        await CardPileCmd.Add(copy, PileType.Hand);
        _usesThisTurn++;
        Flash();
        InvokeDisplayAmountChanged();
    }

    public override Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, CombatState combatState)
    {
        if (player.Creature != Owner) return Task.CompletedTask;
        _usesThisTurn = 0;
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }
}