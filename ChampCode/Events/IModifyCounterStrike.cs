using Champ.ChampCode.Cards.Common;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Champ.ChampCode.Events;

public interface IModifyCounterStrike
{
    bool ModifyCounterStrike(Player player, RiposteStrike card);
    Task AfterModifyingCounterStrike(Player player, RiposteStrike card);
}