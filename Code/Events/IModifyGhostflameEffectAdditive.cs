using Downfall.Code.Core.Hexaghost;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Events;

public interface IModifyGhostflameEffectAdditive
{
    int ModifyGhostflameEffectAdditive(PlayerChoiceContext ctx, Player owner, GhostflameModel bolsteringGhostflame);
}