using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Downfall.Code.Powers.Awakened;

#pragma warning disable STS001
public class AwakenedFormPower : AwakenedPowerModel
#pragma warning restore STS001
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.None;
    protected override bool IsVisibleInternal => false;

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        var creatureNode = NCombatRoom.Instance?.GetCreatureNode(Owner);
        if (creatureNode == null) return Task.CompletedTask;
        
        creatureNode.SetAnimationTrigger("Idle");
        var current = creatureNode.SpineAnimation.GetAnimationState()?.GetCurrent(0);
        current?.SetMixDuration(0.5f);


        return Task.CompletedTask;
    }
}