using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Downfall.Code.Events;

public interface IAfterCustomDraw
{
    Task AfterCustomDraw(Player player, PileType pile, CardPileAddResult result);
}