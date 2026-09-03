using Collector.CollectorCode.Core;
using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Collector.CollectorCode.Powers;

public class PyromancyPower : CollectorPowerModel
{

    public PyromancyPower()
    {
        WithReserveTip();
        WithTip(CardKeyword.Exhaust);
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext ctx, Player player)
    {
        if (player.Creature != Owner) return;
        var prefs = new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 0, 1);
        var result = (await  CardSelectCmd.FromHand(ctx, player, prefs, null,
            this)).FirstOrDefault();
        if (result == null) return;
        await CardCmd.Exhaust(ctx, result);
        await CollectorCmd.GetReserve(player, Amount);
    }

  
}