using Automaton.AutomatonCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Automaton.AutomatonCode.Powers;

public class RepulsePower : AutomatonPowerModel
{
    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card.Creature != Owner || card.Type != CardType.Status)
            return;
        Flash();
        await CreatureCmd.GainBlock(Owner, Amount, BlockProps.nonCardUnpowered, null);
    }
}