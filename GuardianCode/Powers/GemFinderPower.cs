using Guardian.GuardianCode.Cards.Abstract;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.CustomEnums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Guardian.GuardianCode.Powers;

public class GemFinderPower : GuardianPowerModel
{
    public GemFinderPower()
    {
        WithTip(GuardianKeyword.Gem);
        WithTip(GuardianTip.Socket);
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var card = cardPlay.Card;
        if (card.Owner.Creature != Owner || card is not IGemCard) return;
        await CardPileCmd.Draw(ctx, Amount, card.Owner);
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