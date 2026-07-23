using MegaCrit.Sts2.Core.Entities.Players;

namespace Guardian.GuardianCode.Interfaces;

public interface IAfterBrace
{
    Task AfterBrace(Player player, decimal amount);
}