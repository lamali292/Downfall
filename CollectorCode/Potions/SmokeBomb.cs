using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Powers;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Collector.CollectorCode.Potions;

[Pool(typeof(CollectorPotionPool))]
public class SmokeBomb : CollectorPotionModel
{
    public SmokeBomb() : base(PotionRarity.Uncommon, PotionUsage.CombatOnly, TargetType.AllEnemies)
    {
        WithPower<MiasmaPower>(8);
    }

    protected override async Task OnUse(PlayerChoiceContext ctx, Creature? target)
    {
        if (target?.Player == null) return;
        await MyCommonActions.Apply<MiasmaPower>(ctx, this, Owner.Creature.CombatState?.HittableEnemies);
    }
   
}