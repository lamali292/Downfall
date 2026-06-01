using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Events;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Relics;

[Pool(typeof(SlimeBossRelicPool))]
public class SlimedTail : SlimeBossRelicModel, IAfterSplit
{
    public SlimedTail() : base(RelicRarity.Rare)
    {
        WithBlock(3);
    }


    public Task AfterSplit(Player player, SlimeModel slime)
    {
        return player == Owner ? MyCommonActions.Block(this) : Task.CompletedTask;
    }
}