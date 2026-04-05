using Downfall.Code.Abstract;
using Downfall.Code.Events;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Downfall.Code.Powers.Awakened;

public class ClarionCallPower : AwakenedPowerModel, IOnDrained
{
    public async Task OnDrained(Player player, int amount)
    {
        if (player != Owner.Player) return;
        await PlayerCmd.GainEnergy(Amount, player);
    }
}