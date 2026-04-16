using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Events;

public interface IAfterCustomDraw
{
    Task AfterCustomDraw(Player player, PileType pile, CardPileAddResult result);
}