using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Events;

public interface IModifySelfDamage
{
    decimal ModifySelfDamage(decimal amount, AbstractModel model);
    Task AfterModifyingSelfDamage(AbstractModel model);
}