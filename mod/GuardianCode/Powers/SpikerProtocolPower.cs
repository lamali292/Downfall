using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.Events;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Guardian.GuardianCode.Powers;

public class SpikerProtocolPower : GuardianPowerModel, IAfterGuardianModeChange
{
    public async Task AfterGuardianModeChange(PlayerChoiceContext ctx, Player player, GuardianModeModel oldMode,
        GuardianModeModel newMode)
    {
        if (player.Creature != Owner || newMode is not GuardianDefensiveMode) return;
        await PowerCmd.Apply<ThornsPower>(ctx, Owner, Amount, Owner, null);
    }
}