using MegaCrit.Sts2.Core.Models;

namespace Champ.ChampCode.Events;

/// <summary>Combat hook: return true to let <paramref name="card"/> be played as a Finisher without a Finisher stance.</summary>
public interface IAllowFinisherWithoutStance
{
    bool AllowFinisherWithoutStance(CardModel card);
}
