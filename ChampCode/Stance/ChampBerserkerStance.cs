using Champ.ChampCode.Core;
using Champ.ChampCode.Events;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Champ.ChampCode.Stance;

public class ChampBerserkerStance : ChampStanceModel
{
    internal const int BaseFinisherAmount = 1;
    public override bool ShouldReceiveCombatHooks => true;
    public override bool HasFinisher => true;
    public override string ChargeIconPath => "res://Champ/images/ui/stance_charge_berserker.png";

    public override async Task SkillBonus(PlayerChoiceContext ctx)
    {
        var amount = ChampHook.ModifySkillBonus<VigorPower>(CombatState, this, 2);
        await PowerCmd.Apply<VigorPower>(ctx, Owner.Creature, amount, Owner.Creature, null);
    }

    public override async Task Finisher(PlayerChoiceContext ctx)
    {
        var amount = ChampHook.ModifyBerserkerFinisherBonus(CombatState, this, BaseFinisherAmount);
        await PowerCmd.Apply<StrengthPower>(ctx, Owner.Creature, amount, Owner.Creature, null);
    }
}