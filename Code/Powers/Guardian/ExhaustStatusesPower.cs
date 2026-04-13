using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Powers.Guardian;

public class ExhaustStatusesPower : GuardianPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool ShouldReceiveCombatHooks => true;

    private int _triggers;

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card.Owner != Owner.Player) return;
        if (_triggers >= Amount) return;
        if (card.Type is not (CardType.Status or CardType.Curse)) return;
        
        _triggers++;
        await CardCmd.Exhaust(choiceContext, card);
        await CardPileCmd.Draw(choiceContext, 1, Owner.Player);
    }

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player == Owner.Player)
            _triggers = 0;
        return Task.CompletedTask;
    }
}
