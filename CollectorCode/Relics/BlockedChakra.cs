using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Events;
using Collector.CollectorCode.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Collector.CollectorCode.Relics;

[Pool(typeof(CollectorRelicPool))]
public class BlockedChakra : CollectorRelicModel
{
    public BlockedChakra() : base(RelicRarity.Shop)
    {
        WithKindle(3);
        WithTip(CollectorTip.Kindle);
        //WithEnergy(1);
    }
    private DynamicVar KindleAmount => DynamicVars["KindleAmount"];
    
    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props,
        Creature? dealer, CardModel? cardSource)
    {
        if( target != Owner.Torchhead) return;
        
        if (result.UnblockedDamage > 3)
        {
            var toTake = (int)Math.Ceiling(result.UnblockedDamage / 3.0);
            IEnumerable<DamageResult> damageResults = await CreatureCmd.Damage(choiceContext, Owner.Creature, toTake,
                ValueProp.Unblockable | ValueProp.Unpowered, null, null);
        }
    }

    public override async Task AfterSideTurnStart(CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!participants.Contains(Owner.Creature)) return;
        Flash();
        var dV = (int)KindleAmount.BaseValue;
        await CollectorCmd.SummonTorchhead(new BlockingPlayerChoiceContext(), Owner, dV, this);
    }
    
}