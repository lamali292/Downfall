using BaseLib.Abstracts;
using BaseLib.Extensions;
using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.ValueProps;

namespace Collector.CollectorCode.Powers;

public class VeilOfSmokePower : CollectorPowerModel
{

    public VeilOfSmokePower()
    {
        WithTip(StaticHoverTip.Block);
        WithBlock(0);
    }
    
    
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    public override async Task BeforeSideTurnEnd(PlayerChoiceContext ctx, CombatSide side, IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner)) return;
        await CreatureCmd.GainBlock(Owner, DynamicVars.Block, null);
        await PowerCmd.Decrement(this);
    }

    protected override int? SecondAmount => DynamicVars.Block.IntValue;

    public void SetBlock(int blockIntValue)
    {
        DynamicVars.Block.BaseValue = blockIntValue;
        this.InvokeSilentDisplayAmountChanged();
    }
}