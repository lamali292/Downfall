using Downfall.Code.Events;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Core.Champ;


public class BerserkerStance : StanceModel
{
    public override bool ShouldReceiveCombatHooks => true;

    public override async Task OnEnter(PlayerChoiceContext ctx)
    {
        DownfallMainFile.Logger.Info("Berserker Stance Entered " + Owner.NetId);
    }

    public override async Task OnExit(PlayerChoiceContext ctx)
    {
        DownfallMainFile.Logger.Info("Berserker Stance Existed " + Owner.NetId);
    }


    public override async Task SkillBonus(PlayerChoiceContext ctx)
    {
        var amount = DownfallHook.ModifySkillBonus<VigorPower>(ctx, this, 2);
        await PowerCmd.Apply<VigorPower>(Owner.Creature, amount, Owner.Creature, null);
    }
}
