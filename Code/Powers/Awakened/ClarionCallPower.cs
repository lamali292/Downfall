using Downfall.Code.Abstract;
using Downfall.Code.Interfaces;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Downfall.Code.Powers.Awakened;

public class ClarionCallPower : AwakenedPowerModel, IOnDrained
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;


    public async Task OnDrained(Player player, DrainedPower drainedPower, int amount)
    {
        if (player != Owner.Player) return;
        await PlayerCmd.GainEnergy(Amount, player);
    }
}