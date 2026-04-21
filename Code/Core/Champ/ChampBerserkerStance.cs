using Downfall.Code.Events;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Core.Champ;

public class ChampBerserkerStance : ChampStanceModel
{
    internal const int BaseFinisherAmount = 1;
    public override bool ShouldReceiveCombatHooks => true;
    public override bool HasFinisher => true;
    public override string ChargeIconPath => "res://Downfall/images/ui/stance_charge_berserker.png";

    public override async Task SkillBonus()
    {
        var amount = DownfallHook.ModifySkillBonus<VigorPower>(CombatState, this, 2);
        await PowerCmd.Apply<VigorPower>(Owner.Creature, amount, Owner.Creature, null);
    }

    public override async Task Finisher(PlayerChoiceContext ctx)
    {
        var amount = DownfallHook.ModifyFinisherBonus(CombatState, this, BaseFinisherAmount);
        await PowerCmd.Apply<StrengthPower>(Owner.Creature, amount, Owner.Creature, null);
    }
}