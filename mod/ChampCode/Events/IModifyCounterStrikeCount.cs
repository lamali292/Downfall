using MegaCrit.Sts2.Core.Entities.Players;

namespace Champ.ChampCode.Events;

public interface IModifyCounterStrikeCount
{
    int ModifyCounterStrikeCount(Player player, int amount);
}