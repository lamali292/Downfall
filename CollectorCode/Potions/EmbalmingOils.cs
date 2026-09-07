using BaseLib.Utils;
using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Collector.CollectorCode.Potions;

[Pool(typeof(CollectorPotionPool))]
public class EmbalmingOils : CollectorPotionModel
{
    public EmbalmingOils() : base(PotionRarity.Rare, PotionUsage.CombatOnly, TargetType.AnyEnemy)
    {
        WithVar("Increase", 3);
    }

    protected override async Task OnUse(PlayerChoiceContext ctx, Creature? target)
    {
        if  (target == null) return;
        var debuffs = target.Powers
            .Where(p => p.TypeForCurrentAmount == PowerType.Debuff)
            .ToList();
        foreach (var power in debuffs)
        {
            await PowerCmd.ModifyAmount(ctx, power, DynamicVars["Increase"].IntValue, power.Applier,null);
        }
    }
}