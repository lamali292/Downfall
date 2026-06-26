using Awakened.AwakenedCode.Core;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Awakened.AwakenedCode.Powers;

public class SplitWidePower() : AwakenedPowerModel(PowerType.Debuff)
{
    public override async Task AfterDamageGiven(PlayerChoiceContext ctx, Creature? dealer,
        DamageResult result, ValueProp props,
        Creature target, CardModel? cardSource)
    {
        if (target != Owner || Applier == null) return;
        await PowerCmd.Apply<SplitWidePowerPower>(ctx, Applier, Amount, Owner, null);
    }
}

public class SplitWidePowerPower : CustomTemporaryPowerModelWrapper<SplitWidePower, StrengthPower>;