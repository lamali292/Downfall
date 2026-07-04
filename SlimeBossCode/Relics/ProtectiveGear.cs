using BaseLib.Utils;
using Downfall.DownfallCode.Events;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Relics;

[Pool(typeof(SlimeBossRelicPool))]
public class ProtectiveGear : SlimeBossRelicModel, IModifySelfDamage
{
    public ProtectiveGear() : base(RelicRarity.Shop)
    {
        WithVar("TackleReduce", 3);
    }

    
    
/*
    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer,
        CardModel? cardSource)
    {
        SlimeBossMainFile.Logger.Info($"{dealer}, {target},  {amount}, {props},  {cardSource}");
        return dealer == Owner.Creature && target == Owner.Creature && cardSource != null && cardSource.Tags.Contains(SlimeBossTag.Tackle)
            ? -DynamicVars["TackleReduce"].BaseValue
            : 0;
    }*/

    public decimal ModifySelfDamage(decimal amount, AbstractModel model)
    {
        var creature = model.GetCreature();
        return creature != Owner.Creature ? amount : Math.Max(0,amount - DynamicVars["TackleReduce"].BaseValue);
    }

    public Task AfterModifyingSelfDamage(AbstractModel model)
    {
        return Task.CompletedTask;
    }
}

