using Awakened.AwakenedCode.Core;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Awakened.AwakenedCode.Relics;

[Pool(typeof(AwakenedRelicPool))]
public class ShreddedDoll() : AwakenedRelicModel(RelicRarity.Starter)
{
    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext ctx, ICombatState combatState)
    {
        if (player != Owner) return;
        if (combatState.RoundNumber == 1)
            await PowerCmd.Apply<RitualPower>(ctx, player.Creature, 1, player.Creature, null);
        Flash();
        await AwakenedCmd.Conjure(Owner, combatState);
    }
}