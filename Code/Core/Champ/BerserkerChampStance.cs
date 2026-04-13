using Downfall.Code.Events;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Core.Champ;


public class BerserkerChampStance : ChampStanceModel
{
    public override bool ShouldReceiveCombatHooks => true;
    public override bool HasFinisher => true;
    public override string ChargeIconPath => "res://Downfall/images/ui/stance_charge_active.png";

    public override async Task SkillBonus()
    {
        var amount = DownfallHook.ModifySkillBonus<VigorPower>(this, 2);
        await PowerCmd.Apply<VigorPower>(Owner.Creature, amount, Owner.Creature, null);
    }

    public override async Task Finisher(PlayerChoiceContext ctx)
    {
        await PowerCmd.Apply<StrengthPower>(Owner.Creature, 1, Owner.Creature, null);
    }

 
}
