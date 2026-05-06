using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Powers;

public class UnendingSupplyPower : SneckoPowerModel
{
    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext,
        ICombatState combatState)
    {
        if (Owner != player.Creature) return;
        var mutableCards = CardFactory.GetDistinctForCombat(player,
            SneckoModel.GetSneckoCards(player),
            Amount,
            player.RunState.Rng.CombatCardGeneration).ToList();
        foreach (var card in mutableCards)
        {
            card.AddKeyword(CardKeyword.Ethereal);
            card.AddKeyword(CardKeyword.Exhaust);
        }

        await CardPileCmd.Add(mutableCards, PileType.Hand);
    }
}