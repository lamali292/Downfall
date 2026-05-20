using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.Events;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Guardian.GuardianCode.Powers;

public class EvasiveProtocolPower : GuardianPowerModel, IOnGuardianModeChange
{
    public async Task OnGuardianModeChange(PlayerChoiceContext ctx, Player player, GuardianModeModel oldMode,
        GuardianModeModel newMode)
    {
        if (player.Creature != Owner || newMode is not GuardianDefensiveMode) return;
        await GuardianCmd.Polish(ctx, Owner, Amount, null);
    }
}