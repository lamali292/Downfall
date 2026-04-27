using Collector.CollectorCode.Core;
using Collector.CollectorCode.Events;
using Downfall.DownfallCode.Interfaces;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Powers;

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

    public override Task BeforeHandDraw(Player player, PlayerChoiceContext ctx, ICombatState combatState)
    {
        if (player.Creature != Owner) return Task.CompletedTask;
        _usesThisTurn = 0;
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }
}