using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.ValueProps;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.CustomEnums;
using SlimeBoss.SlimeBossCode.Events;

namespace SlimeBoss.SlimeBossCode.Powers;

public class GoopArmorPower : SlimeBossPowerModel, IAfterConsumeEffect
{
    public GoopArmorPower()
    {
        WithTip(StaticHoverTip.Block);
        WithTip(SlimeBossTip.Consume);
    }
    
    public Task AfterConsumeEffect(PlayerChoiceContext ctx, Creature creature, AttackCommand command, decimal amount)
     => command.Attacker != Owner ? Task.CompletedTask : 
         CreatureCmd.GainBlock(Owner, Amount, ValueProp.Unpowered, null);
}