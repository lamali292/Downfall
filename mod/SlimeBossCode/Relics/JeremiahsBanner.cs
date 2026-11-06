using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Relics;

[Pool(typeof(SlimeBossRelicPool))]
public class JeremiahsBanner() : SlimeBossRelicModel(RelicRarity.Uncommon)
{
    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext ctx, ICombatState combatState)
    {
        if (Owner.PlayerCombatState is not { TurnNumber: 1 } || player != Owner) return;
        await SlimeBossCmd.IncreaseSlots(player);
        await SlimeBossCmd.SplitRandom(ctx, player, SlimeType.Normal);
    }
}