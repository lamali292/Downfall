using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Awakened.AwakenedCode.History;

public class ChantEntry(
    CardPlay cardPlay,
    int roundNumber,
    CombatSide currentSide,
    CombatHistory history,
    IEnumerable<Player> players)
    : CombatHistoryEntry(cardPlay.Card.Owner.Creature, roundNumber, currentSide, history, players)
{
    public CardPlay CardPlay { get; set; } = cardPlay;
    public override string Description =>
        $"{Actor.Player?.Character.Id.Entry} started chanting {CardPlay.Card.Id.Entry}";
}