using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;

namespace Downfall.Code.Commands;

public class DownfallCardCmd
{
    public static async Task Insert(CardModel card, Player player)
    {
        var copy = player.Creature.CombatState!.CreateCard(card, player);
        var result = await CardPileCmd.AddGeneratedCardToCombat(copy, PileType.Draw, true, CardPilePosition.Random);
        if (result.success)
            CardCmd.PreviewCardPileAdd(result);
    }

    public static async Task Insert(IEnumerable<CardModel> cards, Player player)
    {
        var copies = cards
            .Select(card => player.Creature.CombatState!.CreateCard(card, player))
            .ToList();

        var results = await CardPileCmd.AddGeneratedCardsToCombat(copies, PileType.Draw, true, CardPilePosition.Random);
        CardCmd.PreviewCardPileAdd(results);
    }

    public static async Task GiveCard<T>(Player player, 
        PileType pileType,  
        CardPilePosition position = CardPilePosition.Bottom,
        float animationTime = 0.6f,
        CardPreviewStyle animationStyle = CardPreviewStyle.HorizontalLayout) where T : CardModel
    {
        var card = player.Creature.CombatState!.CreateCard(ModelDb.Card<T>(), player);
        var result = await CardPileCmd.AddGeneratedCardToCombat(card, pileType, true, position);
        if (result.success)
            CardCmd.PreviewCardPileAdd(result, animationTime, animationStyle);
    }
    
    public static async Task GiveCards<T>(Player player, 
        PileType pileType,  
        int count,
        CardPilePosition position = CardPilePosition.Bottom,
        float animationTime = 0.6f,
        CardPreviewStyle animationStyle = CardPreviewStyle.HorizontalLayout) where T : CardModel
    {
        if (count <= 0) return;
        var cardInstances = new List<CardModel>();
        var model = ModelDb.Card<T>();
        for (var i = 0; i < count; i++)
        {
            var card = player.Creature.CombatState!.CreateCard(model, player);
            cardInstances.Add(card);
        }
        var result = await CardPileCmd.AddGeneratedCardsToCombat(cardInstances, pileType, true, position);
        CardCmd.PreviewCardPileAdd(result, animationTime, animationStyle);
    }
    
    public static async Task AutoPlayFromDrawPile(
        PlayerChoiceContext choiceContext,
        Player player,
        int count,
        AutoPlayType autoPlayType = AutoPlayType.Default,
        bool skipXCapture = false)
    {
        if (CombatManager.Instance.IsOverOrEnding)
            return;

        var cards = new List<CardModel>(count);
        var drawPile = PileType.Draw.GetPile(player);
        for (var i = 0; i < count; ++i)
        {
            await CardPileCmd.ShuffleIfNecessary(choiceContext, player);
            if (drawPile.Cards.Count == 0) break;
            cards.Add(drawPile.Cards[0]);
        }

        foreach (var card in cards.TakeWhile(card => !card.Owner.Creature.IsDead))
        {
            await CardCmd.AutoPlay(
                choiceContext,
                card,
                target: null,
                type: autoPlayType,
                skipXCapture: skipXCapture,
                skipCardPileVisuals: false);
        }
    }
}