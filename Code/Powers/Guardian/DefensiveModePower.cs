using BaseLib.Extensions;
using Downfall.Code.Abstract;
using Downfall.Code.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Powers.Guardian;

public class DefensiveModePower : GuardianPowerModel
{
    public DefensiveModePower()
    {
        WithPower<ThornsPower>(3);
    }
    
    public override async Task  AfterApplied(Creature? applier, CardModel? cardSource)
    {
        if (Owner.Player == null) return;
        await GuardianCmd.EnterDefensiveMode(Owner.Player);
        await PowerCmd.Apply<ThornsPower>(Owner, DynamicVars.Power<ThornsPower>().BaseValue, Owner, null);
    }

    public override bool ShouldClearBlock(Creature creature)
    {
        return creature != Owner;
    }


    public override async Task AfterRemoved(Creature oldOwner)
    {
        if (oldOwner.Player == null) return;
        await GuardianCmd.LeaveDefensiveMode(oldOwner.Player);
        await PowerCmd.Apply<ThornsPower>(Owner, -DynamicVars.Power<ThornsPower>().BaseValue, Owner, null);
    }

    public override async Task BeforeHandDrawLate(Player player, PlayerChoiceContext choiceContext, CombatState combatState)
    {
        if (player.Creature != Owner) return;
        await PowerCmd.Decrement(this);
    }
}