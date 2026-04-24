using Downfall.Code.Abstract;
using Downfall.Code.Events;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Powers.Awakened;

public class DarkblessedPower : AwakenedPowerModel, IOnDrained
{
    public async Task OnDrained(PlayerChoiceContext ctx, Player player, int amount)
    {
        if (player.Creature != Owner) return;
        await PowerCmd.Apply<StrengthPower>(ctx, player.Creature, Amount * amount, player.Creature, null);
    }
}