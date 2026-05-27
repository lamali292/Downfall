using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.Core;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Automaton.AutomatonCode.Powers;

public class InfiniteBeamsPower : AutomatonPowerModel
{
    public override async Task AfterSideTurnStart(CombatSide side,
        IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (!participants.Contains(Owner) || Owner.Player == null) return;
        await DownfallCardCmd.GiveCards<MinorBeam>(Owner.Player, PileType.Hand, Amount);
    }
}