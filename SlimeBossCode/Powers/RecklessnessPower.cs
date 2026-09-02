using Downfall.DownfallCode.Events;
using MegaCrit.Sts2.Core.Models;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Powers;

public class RecklessnessPower : SlimeBossPowerModel, IModifySelfDamage
{
    public decimal ModifySelfDamage(decimal amount, AbstractModel model)
    {
        return model.Creature == Owner ? amount + Amount : amount;
    }

    public Task AfterModifyingSelfDamage(AbstractModel model)
    {
        Flash();
        return Task.CompletedTask;
    }
}