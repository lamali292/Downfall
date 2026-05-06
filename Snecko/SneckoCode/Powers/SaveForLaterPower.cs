using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Powers;

public class SaveForLaterPower : SneckoPowerModel
{
    public override async Task BeforeFlushLate(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature.CombatState == null) return;
        if (player != Owner.Player || !Hook.ShouldFlush(player.Creature.CombatState, player))
            return;
        var prefs = new CardSelectorPrefs(ModelDb.Power<WellLaidPlansPower>().SelectionScreenPrompt, 0, Amount);
        var list = (await CardSelectCmd.FromHand(choiceContext, Owner.Player, prefs, RetainFilter, this)).ToList();
        if (list.Count == 0)
            return;
        foreach (var cardModel in list)
            cardModel.GiveSingleTurnRetain();
        await PowerCmd.Remove(this);
    }

    private bool RetainFilter(CardModel card)
    {
        return !card.ShouldRetainThisTurn;
    }
}