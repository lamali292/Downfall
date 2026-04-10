using Downfall.Code.Events;
using Downfall.Code.Powers.Champ;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.Code.Core.Champ;

public class DefensiveChampStance : ChampStanceModel
{
    public override bool ShouldReceiveCombatHooks => true;
    public override bool HasFinisher => true;
    public override string ChargeIconPath => "res://Downfall/images/ui/stance_charge_active.png";

    public override async Task SkillBonus()
    {
        var amount = DownfallHook.ModifySkillBonus<CounterPower>(CombatState, this, 2);
        await PowerCmd.Apply<CounterPower>(Owner.Creature, amount, Owner.Creature, null);
    }

    internal const int BaseFinisherAmount = 6;

    public override async Task Finisher(PlayerChoiceContext ctx)
    {
        var amount = DownfallHook.ModifyFinisherBonus(CombatState, this, BaseFinisherAmount);
        await CreatureCmd.GainBlock(Owner.Creature, amount, ValueProp.Unpowered, null);
    }
}