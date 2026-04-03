using Downfall.Code.Abstract;
using Downfall.Code.Events;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Powers.Awakened;

public class DarkblessedPower : AwakenedPowerModel, IOnDrained
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public async Task OnDrained(Player player, int amount)
    {
        if (player.Creature != Owner) return;
        await PowerCmd.Apply<StrengthPower>(player.Creature, Amount * amount, player.Creature, null);
    }
}