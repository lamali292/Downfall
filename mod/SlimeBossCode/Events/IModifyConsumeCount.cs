using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace SlimeBoss.SlimeBossCode.Events;

public interface IModifyConsumeCount
{
    int ModifyConsumeCount(Player player, int amount, CardModel? cardSource);
    Task AfterModifyingConsumeCount(Player player, CardModel? cardSource);
}