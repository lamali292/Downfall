using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Interfaces;

public interface IHasAfterlifeEffect
{
    Task AfterlifeEffect(PlayerChoiceContext ctx, CardPlay cardPlay);
}