using Downfall.Code.Abstract;
using Downfall.Code.Events;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Powers.Champ;

public class DancingMasterPower : ChampPowerModel, IOnFinisher
{
    private bool _usesThisTurn;

    public async Task OnFinisher(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner.Player || _usesThisTurn) return;

        await PlayerCmd.GainEnergy(Amount, cardPlay.Card.Owner);
        await CardPileCmd.Draw(ctx, Amount, cardPlay.Card.Owner);
        Flash();
        _usesThisTurn = true;
    }


    public override Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        if (player.Creature != Owner) return Task.CompletedTask;
        _usesThisTurn = false;
        return Task.CompletedTask;
    }
}