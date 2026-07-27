using Downfall.DownfallCode.Powers;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Events;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Powers;

public class InfernoPotionPower : HexaghostPowerModel, IShouldGhostflameTargetAll
{
    public InfernoPotionPower() : base(PowerType.Buff, PowerStackType.Single)
    {
        WithTip<SoulBurnPower>();
    }

    public bool ShouldGhostflameTargetAll(GhostflameModel ghostflame, GhostflameRepeatType damage)
    {
        return ghostflame.Owner.Creature == Owner;
    }

    public Task AfterShouldGhostflameTargetedAll(PlayerChoiceContext ctx, GhostflameModel ghostflame)
    {
        Flash();
        return Task.CompletedTask;
    }
}