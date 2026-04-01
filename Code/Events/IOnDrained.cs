using MegaCrit.Sts2.Core.Entities.Players;

namespace Downfall.Code.Events;

public interface IOnDrained
{
    Task OnDrained(Player player, int amount)
    {
        return Task.CompletedTask;
    }
}