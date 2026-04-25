using Downfall.Code.Abstract;
using Downfall.Code.Core.Guardian;
using Downfall.Code.Events;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Powers.Guardian;

public class SpikerProtocolPower : GuardianPowerModel, IOnGuardianModeChange
{
    public async Task OnGuardianModeChange(PlayerChoiceContext ctx, Player player, GuardianModeModel oldMode,
        GuardianModeModel newMode)
    {
        if (player.Creature != Owner || newMode is not GuardianDefensiveMode) return;
        await PowerCmd.Apply<ThornsPower>(ctx, Owner, Amount, Owner, null);
    }
}