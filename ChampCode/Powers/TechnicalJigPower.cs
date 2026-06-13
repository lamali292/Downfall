using Champ.ChampCode.Core;
using Champ.ChampCode.Events;
using Champ.ChampCode.Stance;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace Champ.ChampCode.Powers;

public class TechnicalJigPower : ChampPowerModel, IOnChampStanceChange
{
    public async Task OnChampStanceChange(PlayerChoiceContext ctx, Player player, ChampStanceModel oldStance,
        ChampStanceModel newStance)
    {
        if (player.Creature != Owner || newStance is ChampNoStance) return;
        await CreatureCmd.GainBlock(player.Creature, Amount, ValueProp.Unpowered | ValueProp.Move, null);
    }
}