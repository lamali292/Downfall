using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Powers;

public class InfiniteBeamsPower : AutomatonPowerModel
{
    public override async Task AfterSideTurnStart(CombatSide side, ICombatState combatState)
    {
        if (side != Owner.Side || Owner.Player == null) return;

        var beams = Enumerable.Range(0, Amount)
            .Select(CardModel (_) => combatState.CreateCard<MinorBeam>(Owner.Player))
            .ToList();

        await CardPileCmd.AddGeneratedCardsToCombat(beams, PileType.Hand, Owner.Player);
    }
}