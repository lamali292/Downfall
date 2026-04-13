using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Commands;

public record ScryResult(IReadOnlyList<CardModel> Discarded);

public static class ScryCmd
{
    public static event Func<Player, int, Task>? Scryed;

    public static async Task<ScryResult> Execute(PlayerChoiceContext choiceContext, Player player, int amount)
    {
        //if (player.GetRelic<GoldenEye>() != null) amount += 2;

        if (amount <= 0) return new ScryResult([]);

        var drawPile = PileType.Draw.GetPile(player);
        var cardsToScry = drawPile.Cards.Take(amount).ToList();


        if (cardsToScry.Count == 0) return new ScryResult([]);
        var prefs = new CardSelectorPrefs(
            CardSelectorPrefs.DiscardSelectionPrompt,
            0,
            cardsToScry.Count
        );

        var cardsToDiscard = await CardSelectCmd.FromSimpleGrid(
            choiceContext,
            cardsToScry,
            player,
            prefs
        );

        var cardModels = cardsToDiscard.ToList();
        await CardPileCmd.Add(cardModels, PileType.Discard);

        await NotifyPowers(player, amount);
        await NotifyCards(player, amount);
        return new ScryResult(cardModels);
    }


    private static async Task NotifyPowers(Player player, int amount)
    {
        if (Scryed != null) await Scryed.Invoke(player, amount);
    }

    private static async Task NotifyCards(Player? player, int amount)
    {
        if (player?.PlayerCombatState?.AllPiles != null)
            foreach (var pile in player.PlayerCombatState?.AllPiles!)
            {
                var watcherCards = pile.Cards.OfType<WatcherCardModel>().ToList();

                foreach (var card in watcherCards) await card.OnScryed(player, amount);
            }
    }
}