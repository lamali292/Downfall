using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Guardian.GuardianCode.Events;

public interface IAfterCardTick
{
    Task AfterCardTick(PlayerChoiceContext ctx, CardModel card, Player player);
}