using Downfall.Code.Events;
using Downfall.Code.Powers.Champ;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Core.Champ;

public class DefensiveStance : StanceModel
{
    public override bool ShouldReceiveCombatHooks => true;

    public override async Task OnEnter(PlayerChoiceContext ctx)
    {
        DownfallMainFile.Logger.Info("Defensive Stance Entered " + Owner.NetId);
    }

    public override async Task OnExit(PlayerChoiceContext ctx)
    {
        DownfallMainFile.Logger.Info("Defensive Stance Existed " + Owner.NetId);
    }
    
    public override async Task SkillBonus(PlayerChoiceContext ctx)
    {
        var amount = DownfallHook.ModifySkillBonus<CounterPower>(ctx, this, 2);
        await PowerCmd.Apply<CounterPower>(Owner.Creature, amount, Owner.Creature, null);
    }
}
