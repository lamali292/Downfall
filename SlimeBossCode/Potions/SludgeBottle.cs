using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Potions;

[Pool(typeof(SlimeBossPotionPool))]
public class SludgeBottle : SlimeBossPotionModel
{
    public SludgeBottle() : base(PotionRarity.Uncommon, PotionUsage.CombatOnly, TargetType.AllEnemies)
    {
        WithPower<WeakPower>(2);
    }

    protected override Task OnUse(PlayerChoiceContext ctx, Creature? target)
    {
        return MyCommonActions.ApplyToAllEnemies<WeakPower>(ctx, this);
    }
}
