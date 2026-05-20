using Champ.ChampCode.Core;
using Champ.ChampCode.Events;
using Champ.ChampCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Champ.ChampCode.Stance;

public class ChampUltimateStance : ChampStanceModel
{
    public override bool ShouldReceiveCombatHooks => true;
    public override bool HasFinisher => true;
    public override string ChargeIconPath => "res://Champ/images/ui/stance_charge_ultimate.png";

    public override async Task SkillBonus(PlayerChoiceContext ctx)
    {
        var vigor = ChampHook.ModifySkillBonus<VigorPower>(CombatState, this, 2);
        await PowerCmd.Apply<VigorPower>(ctx, Owner.Creature, vigor, Owner.Creature, null);

        var counter = ChampHook.ModifySkillBonus<CounterPower>(CombatState, this, 2);
        await PowerCmd.Apply<CounterPower>(ctx, Owner.Creature, counter, Owner.Creature, null);
    }

    public override async Task Finisher(PlayerChoiceContext ctx)
    {
        var strength = ChampHook.ModifyBerserkerFinisherBonus(CombatState, this, ChampBerserkerStance.BaseFinisherAmount);
        await PowerCmd.Apply<StrengthPower>(ctx, Owner.Creature, strength, Owner.Creature, null);

        var block = ChampHook.ModifyDefensiveFinisherBonus(CombatState, this, ChampDefensiveStance.BaseFinisherAmount);
        await CreatureCmd.GainBlock(Owner.Creature, block, ValueProp.Unpowered, null);
    }
}