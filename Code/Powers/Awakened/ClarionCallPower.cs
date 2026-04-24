using Downfall.Code.Abstract;
using Downfall.Code.Events;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Powers.Awakened;

public class ClarionCallPower : AwakenedPowerModel, IOnDrained
{
    public async Task OnDrained(PlayerChoiceContext ctx, Player player, int amount)
    {
        if (player != Owner.Player) return;
        await PlayerCmd.GainEnergy(Amount, player);
    }
}