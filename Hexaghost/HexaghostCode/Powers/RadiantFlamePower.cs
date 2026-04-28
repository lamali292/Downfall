using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Events;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace Hexaghost.HexaghostCode.Powers;

public class RadiantFlamePower : HexaghostPowerModel, IAfterGhostflameIgnited
{
    public Task AfterGhostflameIgnited(PlayerChoiceContext ctx, Player player, GhostflameModel flame, int index)
        =>CreatureCmd.GainBlock(Owner, Amount, ValueProp.Move | ValueProp.Unpowered, null);
    
}