using Collector.CollectorCode.Cards.Basic;
using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Powers;

public class DarkLordFormPower : CollectorPowerModel
{
    public override async Task BeforeHandDrawLate(Player player, PlayerChoiceContext ctx, ICombatState combatState)
    {
        if (player.Creature != Owner) return;
        for (var i = 0; i < Amount; i++)
        {
            var card = player.Creature.CombatState!.CreateCard(ModelDb.Card<YouAreMine>(), player);
            card.ExhaustOnNextPlay = true;
            await CardCmd.AutoPlay(ctx, card, null);
            await CardPileCmd.RemoveFromCombat(card, true);
        }
    }
}