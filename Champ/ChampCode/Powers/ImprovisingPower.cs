using Champ.ChampCode.Core;
using Champ.ChampCode.Events;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Champ.ChampCode.Powers;

public class ImprovisingPower : ChampPowerModel, IOnChampStanceChange
{
    public async Task OnChampStanceChange(PlayerChoiceContext ctx, Player player, ChampStanceModel oldStance,
        ChampStanceModel newStance)
    {
        if (player.Creature != Owner || newStance is ChampNoStance) return;
        for (var i = 0; i < Amount; i++) await newStance.SkillBonus(ctx);
    }
}