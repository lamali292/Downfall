using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Collector.CollectorCode.Core;

public class CollectorModel() : CustomSingletonModel(HookType.Combat)
{
    public override async Task AfterSideTurnEnd(PlayerChoiceContext ctx, CombatSide side, IEnumerable<Creature> participants)
    {
        foreach (var players in participants.Select(e => e.Player).OfType<Player>())
        {
            await CollectorCmd.TorchheadAttack(ctx, players, 5);
        }

    }
}