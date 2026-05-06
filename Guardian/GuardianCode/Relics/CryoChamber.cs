using BaseLib.Utils;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.Events;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Guardian.GuardianCode.Relics;

[Pool(typeof(GuardianRelicPool))]
public class CryoChamber() : GuardianRelicModel(RelicRarity.Rare), IBeforeCardEntersStasis
{
    
    public Task BeforeCardEntersStasis(PlayerChoiceContext ctx, CardModel card, AbstractModel source)
    {
        if (card.Owner != Owner) return Task.CompletedTask;
        CardCmd.Upgrade(card);
        return Task.CompletedTask;
    }

    public override Task BeforeHandDraw(Player player, PlayerChoiceContext ctx, ICombatState combatState)
    {
        if (player != Owner || combatState.RoundNumber > 1) return Task.CompletedTask;
        GuardianCmd.AddMaxStasisSlots(player);
        return Task.CompletedTask;
    }
}