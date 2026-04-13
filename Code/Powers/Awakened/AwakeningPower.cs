using Downfall.Code.Abstract;
using Downfall.Code.Commands;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;

namespace Downfall.Code.Powers.Awakened;

public class AwakeningPower : AwakenedPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldAllowHitting(Creature creature)
        => creature != Owner || !_isReviving;

    public override bool ShouldCreatureBeRemovedFromCombatAfterDeath(Creature creature)
        => creature != Owner || !_isReviving;

    
    private bool _isReviving;

    public override bool ShouldDie(Creature creature)
    {
        return creature != Owner;
    }

    public override async Task AfterDeath(
        PlayerChoiceContext choiceContext,
        Creature creature,
        bool wasRemovalPrevented,
        float deathAnimLength)
    {
        if (!wasRemovalPrevented || creature != Owner) return;

        _isReviving = true;
        await PowerCmd.Remove<WeakPower>(Owner);
        await PowerCmd.Remove<VulnerablePower>(Owner);
        await PowerCmd.Remove<FrailPower>(Owner);
        await CreatureCmd.Heal(Owner, Amount);
        _isReviving = false;
        
        if (Owner.Player != null) 
            await AwakenedCmd.Awaken(Owner.Player, choiceContext);

        
        await PowerCmd.Remove(this);

       
    }
    
    public override async Task AfterCombatEnd(CombatRoom room)
    {
        if (Owner.IsAlive) return;
        await CreatureCmd.Heal(Owner, Amount);
    }

  
}