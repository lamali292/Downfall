using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Powers;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Collector.CollectorCode.Potions;

[Pool(typeof(CollectorPotionPool))]
public class BottledHex : CollectorPotionModel
{
    public BottledHex() : base(PotionRarity.Common, PotionUsage.CombatOnly, TargetType.AnyEnemy)
    {
        WithPower<WeakPower>(1);
        WithPower<VulnerablePower>(1);
        WithPower<MiasmaPower>(1);
    }

    protected override async Task OnUse(PlayerChoiceContext ctx, Creature? target)
    {
        await MyCommonActions.Apply<WeakPower>(ctx, this, target);
        await MyCommonActions.Apply<VulnerablePower>(ctx, this, target);
        await MyCommonActions.Apply<MiasmaPower>(ctx, this, target);
    }
}