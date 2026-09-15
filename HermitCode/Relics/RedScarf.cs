using Hermit.HermitCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace Hermit.HermitCode.Relics;

/// <summary>
///     Whenever you apply a new debuff to an enemy, gain 3 Block.
/// </summary>
public sealed class RedScarf : HermitRelicModel
{
    // Powers currently mid-application as a brand new enemy debuff from us, recorded in
    // BeforePowerAmountChanged. AfterPowerAmountChanged only fires once modifiers (like Artifact,
    // which zeroes the amount via TryModifyPowerAmountReceived) have had their say, so checking this
    // set there is what tells us whether the debuff actually landed.
    private readonly HashSet<PowerModel> _pendingNewDebuffs = new();

    public RedScarf() : base(RelicRarity.Rare)
    {
        WithBlock(3);
    }

    public override Task BeforePowerAmountChanged(PowerModel power, decimal amount, Creature target,
        Creature? applier, CardModel? cardSource)
    {
        if (amount != 0 && target.IsEnemy && power.GetTypeForAmount(amount) == PowerType.Debuff &&
            (target.GetPower(power.Id)?.Amount ?? 0) == 0 && applier == Owner.Creature)
        {
            _pendingNewDebuffs.Add(power);
        }

        return Task.CompletedTask;
    }

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power,
        decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (!_pendingNewDebuffs.Remove(power)) return;

        Flash();
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, null);
    }

    // A debuff flagged in BeforePowerAmountChanged that then gets fully blocked (Artifact, or any
    // other effect that zeroes the amount) never reaches AfterPowerAmountChanged, so its entry would
    // otherwise never be removed. This relic instance lives for the whole run, not just one combat,
    // so bound the leak by dropping anything still pending once the combat it applied in is over.
    public override Task AfterCombatEnd(CombatRoom room)
    {
        _pendingNewDebuffs.Clear();
        return base.AfterCombatEnd(room);
    }
}
