using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Events;

public interface IAfterCommand
{
    /// <param name="source">The card whose effect triggered this Command, or null if it wasn't card-triggered
    /// (a relic/power effect, or the automatic end-of-turn Command - see <paramref name="isAutomatic"/>).</param>
    /// <param name="isAutomatic">True for the automatic end-of-turn Command every slime performs
    /// (SlimeBossModel.BeforeSideTurnEnd), false for any Command triggered by a card, power, or relic.</param>
    Task AfterCommand(PlayerChoiceContext ctx, Player player, SlimeModel slime, CardModel? source, bool isAutomatic);
}
