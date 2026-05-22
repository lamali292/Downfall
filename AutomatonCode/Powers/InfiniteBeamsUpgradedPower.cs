using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.Core;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Powers;

[Obsolete]
public class InfiniteBeamsUpgradedPower : AutomatonPowerModel
{
    protected override async Task AfterSideTurnStart(PlayerChoiceContext ctx, CombatSide side,
        IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != Owner.Side || Owner.Player == null) return;
        await DownfallCardCmd.GiveCards<MinorBeam>(Owner.Player, PileType.Hand, Amount, upgraded: true);
    }
}