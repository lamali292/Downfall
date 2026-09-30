using MegaCrit.Sts2.Core.Models;

namespace Champ.ChampCode.Events;

/// <summary>Combat hook: return true to keep the current stance after <paramref name="card"/> plays its Finisher.</summary>
public interface IKeepStanceAfterFinisher
{
    bool KeepStanceAfterFinisher(CardModel card);
}
