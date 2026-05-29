using Automaton.AutomatonCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Automaton.AutomatonCode.Powers;

public class RepulsePower : AutomatonPowerModel
{
    public override async Task AfterCardGeneratedForCombat(CardModel card, Player? creator)
    {
        if (creator == null || creator.Creature != Owner || card.Type != CardType.Status)
            return;
        Flash();
        await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Move | ValueProp.Unpowered, null);
    }
}