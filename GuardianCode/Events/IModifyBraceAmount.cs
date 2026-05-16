using MegaCrit.Sts2.Core.Entities.Players;

namespace Guardian.GuardianCode.Events;

public interface IModifyBraceAmount
{
    decimal ModifyBraceAmount(Player player, decimal amount);
}