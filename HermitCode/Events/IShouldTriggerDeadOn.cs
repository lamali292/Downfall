using MegaCrit.Sts2.Core.Models;

namespace Hermit.HermitCode.Events;

public interface IShouldTriggerDeadOn
{
    bool ShouldTriggerDeadOn(CardModel card);
}