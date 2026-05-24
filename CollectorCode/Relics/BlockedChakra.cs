using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Events;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Collector.CollectorCode.Relics;

[Pool(typeof(CollectorRelicPool))]
public class BlockedChakra : CollectorRelicModel, IPreventCollectedDraw
{
    public BlockedChakra() : base(RelicRarity.Ancient)
    {
        WithEnergy(1);
    }

    public bool PreventCollectedDraw(Player player)
    {
        if (player != Owner) return false;
        return player.Creature.CombatState is { RoundNumber: <= 4 };
    }

    protected override async Task AfterSideTurnStart(PlayerChoiceContext ctx, CombatSide side, IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (side != Owner.Creature.Side)
            return;
        Flash();
        await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);
    }
}