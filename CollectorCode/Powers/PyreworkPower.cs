using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Collector.CollectorCode.Powers;

public class PyreworkPower : CollectorPowerModel
{

    public PyreworkPower()
    {
        WithReserveTip();
        WithTip(CardKeyword.Exhaust);
        WithTorchheadDamage(5);//If this value changes, change the value in the main thing too.
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner || !(cardPlay.Card.Keywords.Contains(CollectorKeyword.Pyre) || cardPlay.Card.Keywords.Contains(CollectorKeyword.Megapyre))) return;
        for (int i = 0; i < Amount; i++)
        {
            await CollectorCmd.TorchheadAttack(ctx, Owner.Player!, DynamicVars.TorchheadDamage.IntValue);
        }
    }
    
    /*
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext ctx, Player player)
    {//Hmm how to "pyre" from inside a power?
        if (player.Creature != Owner) return;
        var prefs = new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 0, 1);
        var result = (await  CardSelectCmd.FromHand(ctx, player, prefs, null,
            this)).FirstOrDefault();
        if (result == null) return;
        await CardCmd.Exhaust(ctx, result);
        await CollectorCmd.GetReserve(player, Amount);
    }
    */

  
}