using Downfall.Code.Abstract;
using Downfall.Code.Cards.Collector.Basic;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Powers.Collector;

public class DarkLordFormPlusPower : CollectorPowerModel
{
    public override async Task BeforeHandDrawLate(Player player, PlayerChoiceContext ctx, CombatState combatState)
    {
        if (player.Creature != Owner) return;
        for (var i = 0; i < Amount; i++)
        {
            var card = player.Creature.CombatState!.CreateCard(ModelDb.Card<YouAreMine>(), player);
            card.UpgradeInternal();
            card.ExhaustOnNextPlay = true;
            await CardCmd.AutoPlay(ctx, card, null);
            await CardPileCmd.RemoveFromCombat(card, true);
        }
    }
}