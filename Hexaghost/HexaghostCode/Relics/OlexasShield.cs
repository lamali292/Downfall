using BaseLib.Utils;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Events;
using Hexaghost.HexaghostCode.Ghostflames;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace Hexaghost.HexaghostCode.Relics;

[Pool(typeof(HexaghostRelicPool))]
public class OlexasShield() : HexaghostRelicModel(RelicRarity.Uncommon), IGhostflameConditionOverwrites
{
    
    public bool GhostflameConditionOverwrites(Player player, GhostflameModel ghostflame, CardPlay cardPlay)
    {
        return player == Owner && ghostflame is SearingGhostflame or CrushingGhostflame &&
               cardPlay.Card.Type == CardType.Power;
    }
}