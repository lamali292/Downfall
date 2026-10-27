using Collector.CollectorCode.Cards.Collectibles.Ruina2;
using Collector.CollectorCode.Core;
using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Collector.CollectorCode.Powers;

public class EternallyLitLampPower : CollectorPowerModel
{
    public EternallyLitLampPower()
    {
        WithTip<StrengthPower>();
    }

    public override async Task AfterDamageGiven(
        PlayerChoiceContext choiceContext,
        Creature? dealer,
        DamageResult _,
        ValueProp props,
        Creature target,
        CardModel? cardSource)
    {
        if (dealer != null && dealer.Monster is TorchheadMonsterModel && dealer.PetOwner != null && dealer.PetOwner.Creature == Owner && props.IsPoweredAttack())
        {
            await PowerCmd.Apply<EternallyLitLampPowerStrengthDownPower>(choiceContext, target, Amount, Owner, null);
        }
    }
}
public class EternallyLitLampPowerStrengthDownPower : TemporaryDebuffPowerWrapper<EternallyLitLamp, StrengthPower>;