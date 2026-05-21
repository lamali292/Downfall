using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace Awakened.AwakenedCode.Events;

public interface IModifyBaseSpells
{
    IReadOnlyList<CardModel> ModifyBaseSpells(Player owner, IReadOnlyList<CardModel> types);
}

