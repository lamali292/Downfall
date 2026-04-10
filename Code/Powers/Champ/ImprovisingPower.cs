using Downfall.Code.Abstract;
using Downfall.Code.Core.Champ;
using Downfall.Code.Events;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Powers.Champ;

public class ImprovisingPower : ChampPowerModel, IOnStanceChange
{
    public async Task OnStanceChange(PlayerChoiceContext ctx, Player player, ChampStanceModel oldStance,
        ChampStanceModel newStance)
    {
        if (player.Creature != Owner || newStance is NoChampStance) return;
        for (var i = 0; i < Amount; i++) await newStance.SkillBonus();
    }
}