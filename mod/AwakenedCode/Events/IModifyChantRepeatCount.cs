using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace Awakened.AwakenedCode.Events;

public interface IModifyChantRepeatCount
{
    int ModifyChantRepeatCount(CardModel card, CardPlay cardPlay, int count);
    Task AfterModifyingChantRepeatCount(CardModel card, CardPlay cardPlay);
}
