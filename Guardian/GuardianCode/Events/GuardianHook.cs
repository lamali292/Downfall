using Downfall.DownfallCode.Events;
using Guardian.GuardianCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Guardian.GuardianCode.Events;

public static class GuardianHook
{
  

    public static Task OnGuardianModeChange(ICombatState cs, PlayerChoiceContext ctx, Player player,
        GuardianModeModel oldMode,
        GuardianModeModel newMode)
    {
        return DownfallHook.Dispatch<IOnGuardianModeChange>(cs, m => m.OnGuardianModeChange(ctx, player, oldMode, newMode));
    }
    

    public static Task BeforeCardEntersStasis(ICombatState cs, PlayerChoiceContext ctx, CardModel card,
        AbstractModel source)
    {
        return DownfallHook.Dispatch<IBeforeCardEntersStasis>(cs, ctx, m => m.BeforeCardEntersStasis(ctx, card, source));
    }

   

 
}