using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Slimes;
using SlimeBoss.SlimeBossCode.CustomEnums;

namespace SlimeBoss.SlimeBossCode.Relics;

[Pool(typeof(SlimeBossRelicPool))]
public class MagnificentBowlerHat : SlimeBossRelicModel
{
    public MagnificentBowlerHat() : base(RelicRarity.Starter)
    {
        WithTip(SlimeBossTip.Split);
        WithSlimeTip<BruiserSlime>();
    }

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext ctx, ICombatState combatState)
    {
        if (Owner.PlayerCombatState is not { TurnNumber: 1 } || player != Owner) return;
        await SlimeBossCmd.Split<BruiserSlime>(ctx, player);
    }

    public override RelicModel GetUpgradeReplacement()
    {
        return ModelDb.Relic<VeryMagnificentBowlerHat>();
    }
}