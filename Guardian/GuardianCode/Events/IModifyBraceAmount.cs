using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Guardian.GuardianCode.Events;

public interface IModifyBraceAmount
{
    decimal ModifyBraceAmount(Player player, decimal amount);
}