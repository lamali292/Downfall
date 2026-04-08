using Downfall.Code.Abstract;
using Downfall.Code.Core.Champ;
using Downfall.Code.Events;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.Code.Powers.Champ;

public class TechnicalJigPower : ChampPowerModel, IOnStanceChange
{
    public async Task OnStanceChange(PlayerChoiceContext ctx, Player player, ChampStanceModel oldStance,
        ChampStanceModel newStance)
    {
        if (player.Creature != Owner) return;
        await CreatureCmd.GainBlock(player.Creature, Amount, ValueProp.Unpowered | ValueProp.Move, null);
    }
}