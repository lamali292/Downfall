using Awakened.AwakenedCode.Core;
using BaseLib.Abstracts;
using BaseLib.Patches.Localization;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Awakened.AwakenedCode.Powers;

public class SplitWidePower() : AwakenedPowerModel(PowerType.Debuff), IAddDumbVariablesToPowerDescription
{
    public override PowerInstanceType InstanceType => PowerInstanceType.InstancedPerApplier;


    public override async Task AfterDamageGiven(PlayerChoiceContext ctx, Creature? dealer,
        DamageResult result, ValueProp props,
        Creature target, CardModel? cardSource)
    {
        if (target != Owner || dealer == null || dealer != Applier) return;
        await PowerCmd.Apply<SplitWidePowerPower>(ctx, dealer, Amount, Owner, null);
    }

    public void AddDumbVariablesToPowerDescription(LocString description)
    {
        description.Add("IsApplierYou", LocalContext.IsMe(Applier));
    }
}

public class SplitWidePowerPower : CustomTemporaryPowerModelWrapper<SplitWidePower, StrengthPower>;