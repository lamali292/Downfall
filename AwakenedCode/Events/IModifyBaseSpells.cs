using MegaCrit.Sts2.Core.Entities.Players;

namespace Awakened.AwakenedCode.Events;

public interface IModifyBaseSpells
{
    IReadOnlyList<Type> ModifyBaseSpells(Player owner, IReadOnlyList<Type> types);
}

