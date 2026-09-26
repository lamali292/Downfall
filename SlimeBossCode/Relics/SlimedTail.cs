using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.CustomEnums;
using SlimeBoss.SlimeBossCode.Events;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Relics;

[Pool(typeof(SlimeBossRelicPool))]
public class SlimedTail : SlimeBossRelicModel, IAfterCommand
{
    public SlimedTail() : base(RelicRarity.Rare)
    {
        WithTip(SlimeBossTip.Command);
        WithSlimeTip<BruiserSlime>();
        WithBlock(3);
    }


    public Task AfterCommand(PlayerChoiceContext ctx, Player player, SlimeModel slime, CardModel? source)
    {
        return player == Owner && slime is BruiserSlime ? MyCommonActions.Block(this) : Task.CompletedTask;
    }
}