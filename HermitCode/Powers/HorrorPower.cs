using Hermit.HermitCode.Core;
using Hermit.HermitCode.Events;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Powers;


public sealed class HorrorPower() : HermitPowerModel(PowerType.Debuff), IShouldPreventBruiseRemoval
{
    public override async Task AfterTurnEndLate(PlayerChoiceContext ctx, CombatSide side)
    {
        if (side != Owner.Side)return; 
        await PowerCmd.Apply<HorrorPower>(ctx, Owner, -1, Owner, null);
    }
    
    public bool ShouldPreventBruiseRemoval(BruisePower power) =>  power.Owner == Owner;
}