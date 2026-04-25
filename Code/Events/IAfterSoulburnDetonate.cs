using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Events;

public interface IAfterSoulburnDetonate
{
    Task AfterSoulburnDetonate(PlayerChoiceContext ctx, Creature creature);
}