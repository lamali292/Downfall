using Downfall.DownfallCode.Events;
using Hermit.HermitCode.Core;
using Hermit.HermitCode.Events;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Hermit.HermitCode.Powers;

public sealed class BruisePower() : HermitPowerModel(PowerType.Debuff)
{

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer,
        CardModel? cardSource)
        => target != Owner || !props.HasFlag(ValueProp.Move) ? 0 : Amount;

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (HermitHook.ShouldPreventBruiseRemoval(CombatState, this, out var preventers))
        {
            await HermitHook.AfterPreventedBruiseRemoval(CombatState, this, preventers);
            return;
        }
        await PowerCmd.Remove(this);
    }
}

