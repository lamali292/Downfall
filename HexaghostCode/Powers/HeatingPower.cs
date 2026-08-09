using Downfall.DownfallCode.Powers;
using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Hexaghost.HexaghostCode.Powers;

public class HeatingPower : HexaghostPowerModel
{
    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier,
        CardModel? cardSource)
    {
        if (power is not SoulBurnPower || applier != Owner) return;
        var player = CombatState.GetTeammatesOf(Owner)
            .Where(e => e != Owner && e is { IsPlayer: true, IsAlive: true })
            .OrderBy(e => e.Block)
            .FirstOrDefault();
        if (player == null) return;
        await CreatureCmd.GainBlock(player, Amount, BlockProps.nonCardUnpowered, null);
        Flash();
    }
}