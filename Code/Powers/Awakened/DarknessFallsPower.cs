using Downfall.Code.Abstract;
using Downfall.Code.Events;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.Code.Powers.Awakened;

public class DarknessFallsPower : AwakenedPowerModel, IOnDrained
{
    public async Task OnDrained(Player player, int amount)
    {
        if (player.Creature != Owner) return;
        await CreatureCmd.GainBlock(player.Creature, Amount * amount, ValueProp.Unpowered, null);
    }
}