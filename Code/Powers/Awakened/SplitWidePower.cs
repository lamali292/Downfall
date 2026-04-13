using Downfall.Code.Abstract;
using Downfall.Code.Powers.Downfall;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.Code.Powers.Awakened;

public class SplitWidePower() : AwakenedPowerModel(PowerType.Debuff)
{
    public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer,
        DamageResult result, ValueProp props,
        Creature target, CardModel? cardSource)
    {
        if (target != Owner || Applier == null) return;
        await PowerCmd.Apply<TemporaryStrengthUpPower>(Applier, Amount, Owner, null);
    }
}