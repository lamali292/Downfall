using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.Events;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Guardian.GuardianCode.Powers;

public class RevengeProtocolPower : GuardianPowerModel, IOnGuardianModeChange
{
    public RevengeProtocolPower()
    {
        WithTip(typeof(StrengthPower));
    }

    public async Task OnGuardianModeChange(PlayerChoiceContext ctx, Player player, GuardianModeModel oldMode,
        GuardianModeModel newMode)
    {
        if (player.Creature != Owner) return;
        if (!GuardianCmd.IsInMode<GuardianDefensiveMode>(player)) return;
        await PowerCmd.Apply<StrengthPower>(ctx, Owner, Amount, Owner, null);
    }
}