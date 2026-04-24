using Downfall.Code.Events;
using Downfall.Code.Powers.Champ;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.Code.Core.Champ;

public class ChampUltimateStance : ChampStanceModel
{
    public override bool ShouldReceiveCombatHooks => true;
    public override bool HasFinisher => true;
    public override string ChargeIconPath => "res://Downfall/images/ui/stance_charge_ultimate.png";

    public override async Task SkillBonus(PlayerChoiceContext ctx)
    {
        var vigor = DownfallHook.ModifySkillBonus<VigorPower>(CombatState, this, 2);
        await PowerCmd.Apply<VigorPower>(ctx, Owner.Creature, vigor, Owner.Creature, null);

        var counter = DownfallHook.ModifySkillBonus<CounterPower>(CombatState, this, 2);
        await PowerCmd.Apply<CounterPower>(ctx, Owner.Creature, counter, Owner.Creature, null);
    }

    public override async Task Finisher(PlayerChoiceContext ctx)
    {
        var strength = DownfallHook.ModifyFinisherBonus(CombatState, this, ChampBerserkerStance.BaseFinisherAmount);
        await PowerCmd.Apply<StrengthPower>(ctx, Owner.Creature, strength, Owner.Creature, null);

        var block = DownfallHook.ModifyFinisherBonus(CombatState, this, ChampDefensiveStance.BaseFinisherAmount);
        await CreatureCmd.GainBlock(Owner.Creature, block, ValueProp.Unpowered, null);
    }
}