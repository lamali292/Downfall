using MegaCrit.Sts2.Core.Entities.Players;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Events;

public interface IAfterSplit
{
    Task AfterSplit(Player player, SlimeModel slime);
}