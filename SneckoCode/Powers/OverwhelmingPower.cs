using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Powers;

public class OverwhelmingPower : SneckoPowerModel
{
    private bool _usedThisTurn;

    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner || _usedThisTurn || Owner.Player == null ||
            !SneckoCmd.IsOffclass(Owner.Player, cardPlay.Card)) return;
        _usedThisTurn = true;
        await CardPileCmd.Draw(ctx, Amount, Owner.Player);
        Flash();
    }

    public override Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        if (player.Creature != Owner) return Task.CompletedTask;
        _usedThisTurn = false;
        return Task.CompletedTask;
    }
}