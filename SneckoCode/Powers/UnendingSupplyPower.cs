using Downfall.DownfallCode.CustomEnums;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.CustomEnums;

namespace Snecko.SneckoCode.Powers;

public class UnendingSupplyPower : SneckoPowerModel
{

    public UnendingSupplyPower()
    {
        WithTip(DownfallKeyword.Echo);
        WithTip(CardKeyword.Ethereal);
        WithTip(CardKeyword.Exhaust);
        WithTip(SneckoTip.Offclass);
    }
    
    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext,
        ICombatState combatState)
    {
        if (Owner != player.Creature) return;
        var mutableCards = SneckoModel.GetCombatSneckoCards(player, Amount).ToList();
        foreach (var card in mutableCards) card.ToEcho();

        await CardPileCmd.Add(mutableCards, PileType.Hand);
    }
}