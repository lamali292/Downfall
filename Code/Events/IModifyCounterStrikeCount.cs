using MegaCrit.Sts2.Core.Entities.Players;

namespace Downfall.Code.Events;

public interface IModifyCounterStrikeCount
{
    int ModifyCounterStrikeCount(Player player, int amount);
}