using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Events;

public interface IAfterSplit
{
    Task AfterSplit(PlayerChoiceContext ctx, Player player, SlimeModel slime);
}