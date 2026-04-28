using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Hexaghost.HexaghostCode.Events;

public interface IGhostflameConditionOverwrites
{
    bool GhostflameConditionOverwrites(Player player, GhostflameModel ghostflame, CardPlay cardPlay);
}