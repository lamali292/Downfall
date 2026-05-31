using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.CustomEnums;
using Guardian.GuardianCode.Interfaces;
using Guardian.GuardianCode.Rewards;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Rooms;

namespace Guardian.GuardianCode.Powers;

public class GemFinderPower : GuardianPowerModel
{

    public GemFinderPower()
    {
        WithTip(GuardianKeyword.Gem);
        WithTip(GuardianTip.Brace);
    }
    
    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var card = cardPlay.Card;
        if (card.Owner.Creature != Owner || card is not IGemSocketCard gemCard || gemCard.GemCount == 0) return;
        await GuardianCmd.Brace(ctx, card.Owner, Amount);
    }


    /*
     DESCRIPTION:
    At the end of combat you may add a random [gold]Gem[/gold] to your [gold]Deck[/gold].

    SMARTDESCRIPTION:
    At the end of combat you may add {Amount:plural:a random [gold]Gem[/gold]|[blue]{Amount}[/blue] random [gold]Gems[/gold]} to your [gold]Deck[/gold].
    public override Task AfterCombatEnd(CombatRoom room)
    {
        var player = Owner.Player;
        if (player == null) return Task.CompletedTask;
        var specialCardReward = new GemFinderReward(Amount, player);
        room.AddExtraReward(player, specialCardReward);
        return Task.CompletedTask;
    }*/
}