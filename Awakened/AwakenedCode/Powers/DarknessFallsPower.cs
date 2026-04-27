using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Events;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace Awakened.AwakenedCode.Powers;

public class DarknessFallsPower : AwakenedPowerModel, IOnDrained
{
    public async Task OnDrained(PlayerChoiceContext ctx, Player player, int amount)
    {
        if (player.Creature != Owner) return;
        await CreatureCmd.GainBlock(player.Creature, Amount * amount, ValueProp.Unpowered, null);
    }
}