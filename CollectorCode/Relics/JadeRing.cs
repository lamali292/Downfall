using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Powers;
using Collector.CollectorCode.Powers;
﻿using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Events;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
namespace Collector.CollectorCode.Relics;

[Pool(typeof(CollectorRelicPool))]
public class JadeRing() : CollectorRelicModel(RelicRarity.Rare)
{
    public override async Task AfterSideTurnEnd(PlayerChoiceContext ctx, CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side == Owner.Creature.Side) return;
        foreach (var enemy in participants.Where(e => e.IsAlive && e.HasPower<BlisterPower>()))
        {
            var uniqueDebuffs = enemy.Powers
                .Where(p => p.TypeForCurrentAmount == PowerType.Debuff)
                .Select(p => p.Id)
                .Distinct()
                .Count(); // powers that don't stack are only counted once
            if (uniqueDebuffs > 0) await PowerCmd.Apply<BlisterPower>(ctx, enemy, uniqueDebuffs, Owner.Creature, null);
        }
    }
}
