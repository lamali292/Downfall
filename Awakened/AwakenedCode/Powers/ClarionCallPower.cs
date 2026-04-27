using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Events;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Awakened.AwakenedCode.Powers;

public class ClarionCallPower : AwakenedPowerModel, IOnDrained
{
    public async Task OnDrained(PlayerChoiceContext ctx, Player player, int amount)
    {
        if (player != Owner.Player) return;
        await PlayerCmd.GainEnergy(Amount, player);
    }
}