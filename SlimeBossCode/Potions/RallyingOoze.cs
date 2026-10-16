using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Powers;

namespace SlimeBoss.SlimeBossCode.Potions;

[Pool(typeof(SlimeBossPotionPool))]
public class RallyingOoze : SlimeBossPotionModel
{
    public RallyingOoze() : base(PotionRarity.Common, PotionUsage.CombatOnly, TargetType.AnyPlayer)
    {
        WithPower<PotencyPower>(2);
    }

    protected override Task OnUse(PlayerChoiceContext ctx, Creature? target)
    {
        AssertValidForTargetedPotion(target);
        return MyCommonActions.Apply<PotencyPower>(ctx, this, target);
    }
}
