using BaseLib.Patches.Localization;
using Downfall.DownfallCode.Compatibility;
using Hermit.HermitCode.Core;
using Hermit.HermitCode.Events;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Hermit.HermitCode.Powers;

public sealed class BruisePower()
    : HermitPowerModel(PowerType.Debuff), IAddDumbVariablesToPowerDescription, IModifyDamageAdditive
{
    private bool HasBigBruiser => Applier?.HasPower<BigBruiserPower>() ?? false;


    public override PowerInstanceType InstanceType => PowerInstanceType.InstancedPerApplier;

    public void AddDumbVariablesToPowerDescription(LocString description)
    {
        description.Add("HasBigBruiser", HasBigBruiser);
    }

    public decimal ModifyDamageAdditiveCompability(Creature? target, decimal amount, ValueProp props, Creature? dealer,
        CardModel? cardSource, CardPlay? cardPlay)
    {
        return target != Owner || !(dealer == Applier || HasBigBruiser) || !props.IsPoweredAttack() ? 0 : Amount;
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (HermitHook.ShouldPreventBruiseRemoval(CombatState, this, out var preventers))
        {
            await HermitHook.AfterPreventedBruiseRemoval(CombatState, this, preventers);
            return;
        }

        await PowerCmd.Remove(this);
    }
}