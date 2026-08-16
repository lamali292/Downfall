using Automaton.AutomatonCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Powers;

public class LibraryPower : AutomatonPowerModel
{
    public override async Task AfterSideTurnStart(CombatSide side,
        IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (!participants.Contains(Owner) || Owner.Player == null) return;
        var player = Owner.Player;
        var choice = AutomatonCmd.GetEncodableCards(player, Amount).Select(t =>
        {
            t.SetToFreeThisTurn();
            return t;
        });
        await CardPileCmd.AddGeneratedCardsToCombat(choice, PileType.Hand, player);
        Flash();
    }
}