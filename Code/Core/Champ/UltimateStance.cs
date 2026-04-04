using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Core.Champ;

public class UltimateStance : StanceModel
{
    public override bool ShouldReceiveCombatHooks => true;

    public override async Task OnEnter(PlayerChoiceContext ctx)
    {

    }

    public override async Task OnExit(PlayerChoiceContext ctx)
    {
    }
}