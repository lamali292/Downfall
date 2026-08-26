using Downfall.DownfallCode.Compatibility;
using Downfall.DownfallCode.Events;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.CustomEnums;

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