using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Events;

public interface IAfterCommand
{
    Task AfterCommand(PlayerChoiceContext ctx, Player player, SlimeModel slime, CardModel? source);
}
