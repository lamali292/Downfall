using Downfall.Code.Abstract;
using Downfall.Code.Commands;
using Downfall.Code.Core.Guardian;
using Downfall.Code.Events;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Downfall.Code.Powers.Guardian;

public class EvasiveProtocolPower : GuardianPowerModel, IOnGuardianModeChange
{
    public async Task OnGuardianModeChange(Player player, GuardianModeModel oldMode, GuardianModeModel newMode)
    {
        if (player.Creature != Owner || newMode is not GuardianDefensiveMode) return;
        await GuardianCmd.DebuffDown(Owner, Amount);
    }
}