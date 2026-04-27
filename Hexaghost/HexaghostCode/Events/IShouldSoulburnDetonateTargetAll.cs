using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Events;

public interface IShouldSoulburnDetonateTargetAll
{
    bool ShouldSoulburnDetonateTargetAll(PlayerChoiceContext ctx, Creature owner);
}