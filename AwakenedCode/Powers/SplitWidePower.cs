using Awakened.AwakenedCode.Core;
using Downfall.DownfallCode.Powers;

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Awakened.AwakenedCode.Powers;

public class SplitWidePower() : AwakenedPowerModel(PowerType.Debuff)
{
    public override async Task AfterDamageGiven(PlayerChoiceContext ctx, Creature? dealer,
        DamageResult result, ValueProp props,
        Creature target, CardModel? cardSource)
    {
        if (target != Owner || Applier == null) return;
        await PowerCmd.Apply<TemporaryStrengthUpPower>(ctx, Applier, Amount, Owner, null);
    }
}