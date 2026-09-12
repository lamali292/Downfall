using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Collector.CollectorCode.Relics;

[Pool(typeof(CollectorRelicPool))]
public class BlockedChakra : CollectorRelicModel
{
    public BlockedChakra() : base(RelicRarity.Shop)
    {
        WithKindle(3);
     
        //WithEnergy(1);
    }

    public override async Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        if(creature != Owner.Torchhead || delta >= 0) return;
        var toTake = (int)Math.Ceiling(-delta / 3);
        if (toTake <= 0) return;
        Flash();
        await CreatureCmd.Damage(new BlockingPlayerChoiceContext(), Owner.Creature, toTake,
            DamageProps.nonCardHpLoss, null, null);
    }
    

    public override async Task AfterSideTurnStart(CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!participants.Contains(Owner.Creature)) return;
        Flash();
        await CollectorCmd.Kindle(new BlockingPlayerChoiceContext(), this);
    }
    
}