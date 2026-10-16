using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.CustomEnums;

namespace SlimeBoss.SlimeBossCode.Potions;

[Pool(typeof(SlimeBossPotionPool))]
public class CommandingOrders : SlimeBossPotionModel
{
    public CommandingOrders() : base(PotionRarity.Rare, PotionUsage.CombatOnly, TargetType.AnyPlayer)
    {
        WithTip(SlimeBossTip.Command);
    }

    protected override async Task OnUse(PlayerChoiceContext ctx, Creature? target)
    {
        AssertValidForTargetedPotion(target);
        var player = target!.Player!;
        await SlimeBossCmd.CommandAll(ctx, player);
        await SlimeBossCmd.CommandAll(ctx, player);
    }
}
