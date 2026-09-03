using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Events;

public interface IShouldExhaustPyred
{
    bool ShouldExhaustPyred(CardModel card, CardModel pyred);
}