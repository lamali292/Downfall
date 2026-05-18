using Hermit.HermitCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Hermit.HermitCode.Powers;

/// <summary>
///     Bruised: target takes X more attack damage. Wears off at end of target's turn.
/// </summary>
public sealed class BruisePower() : HermitPowerModel(PowerType.Debuff)
{
    /// <summary>
    ///     When the creature with Bruise is hit by an attack, they take extra damage.
    ///     This returns the delta (amount to add), not the total.
    /// </summary>
    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer,
        CardModel? cardSource)
    {
        // Only increase damage taken by the bruised creature
        if (target != Owner)
            return 0m;

        // Only applies to powered attacks
        if (!props.HasFlag(ValueProp.Move))
            return 0m;

        return Amount;
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == Owner.Side && !Owner.HasPower<HorrorPower>())
            await PowerCmd.Remove(this);
    }
}