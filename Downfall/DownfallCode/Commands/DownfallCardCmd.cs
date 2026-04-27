using Downfall.DownfallCode.Events;
using Godot;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace Downfall.DownfallCode.Commands;

public class DownfallCardCmd
{
    public static async Task Insert(CardModel card, Player player)
    {
        var copy = player.Creature.CombatState!.CreateCard(card, player);
        var result = await CardPileCmd.AddGeneratedCardToCombat(copy, PileType.Draw, player, CardPilePosition.Random);
        if (result.success)
            CardCmd.PreviewCardPileAdd(result);
    }

    public static async Task Insert(IEnumerable<CardModel> cards, Player player)
    {
        var copies = cards
            .Select(card => player.Creature.CombatState!.CreateCard(card, player))
            .ToList();

        var results =
            await CardPileCmd.AddGeneratedCardsToCombat(copies, PileType.Draw, player, CardPilePosition.Random);
        CardCmd.PreviewCardPileAdd(results);
    }

    public static async Task<CardModel> GiveCard<T>(Player player,
        PileType pileType,
        CardPilePosition position = CardPilePosition.Bottom,
        bool upgraded = false,
        float animationTime = 0.6f,
        CardPreviewStyle animationStyle = CardPreviewStyle.HorizontalLayout,
        bool skipAnimation = false) where T : CardModel
    {
        var card = player.Creature.CombatState!.CreateCard(ModelDb.Card<T>(), player);
        if (upgraded) card.UpgradeInternal();
        var result = await CardPileCmd.AddGeneratedCardToCombat(card, pileType, player, position);
        if (!result.success || skipAnimation || pileType == PileType.Hand) return result.cardAdded;
        CardCmd.PreviewCardPileAdd(result, animationTime, animationStyle);
        return result.cardAdded;
    }

    public static async Task<IEnumerable<CardModel>> GiveCards<T>(Player player,
        PileType pileType,
        int count,
        CardPilePosition position = CardPilePosition.Bottom,
        bool upgraded = false,
        float animationTime = 0.6f,
        CardPreviewStyle animationStyle = CardPreviewStyle.HorizontalLayout,
        bool skipAnimation = false) where T : CardModel
    {
        if (count <= 0) return [];
        var cardInstances = new List<CardModel>();
        var model = ModelDb.Card<T>();
        for (var i = 0; i < count; i++)
        {
            var card = player.Creature.CombatState!.CreateCard(model, player);
            if (upgraded) card.UpgradeInternal();
            cardInstances.Add(card);
        }

        var result = await CardPileCmd.AddGeneratedCardsToCombat(cardInstances, pileType, player, position);
        if (skipAnimation || pileType == PileType.Hand) return result.Select(e => e.cardAdded);
        CardCmd.PreviewCardPileAdd(result, animationTime, animationStyle);
        return result.Select(e => e.cardAdded);
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
            await CardCmd.AutoPlay(
                choiceContext,
                card,
                null,
                autoPlayType,
                skipXCapture);
    }


    public static async Task AnimateCardFromRewardScreen(Vector2 targetPos, CardModel card, Player player)
    {
        var node = NCard.Create(card);
        if (node == null) return;
        var previewContainer = NRun.Instance?.GlobalUi.CardPreviewContainer;
        var trailContainer = NRun.Instance?.GlobalUi.TopBar.TrailContainer;
        if (previewContainer == null || trailContainer == null) return;
        previewContainer.AddChildSafely(node);
        var tween = node.CreateTween();
        tween.TweenProperty(node, "scale", Vector2.One, 0.25f)
            .From(Vector2.Zero)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Cubic);
        await node.ToSignal(tween, Tween.SignalName.Finished);
        var fly = NCardFlyVfx.Create(node, targetPos, true, player.Character.TrailPath);
        trailContainer.AddChildSafely(fly);
        if (fly != null)
            await fly.ToSignal(fly, Node.SignalName.TreeExited);
    }


    public static async Task<CardPileAddResult> DrawFromCustomPile(PlayerChoiceContext ctx, Player player,
        PileType pileType)
    {
        if (player.Creature.CombatState == null) return default;
        var pile = pileType.GetPile(player);
        CardPileAddResult result;
        if (pile.Cards.Count == 0)
        {
            result = new CardPileAddResult();
        }
        else
        {
            var cardsToDraw = pile.Cards[0];
            result = await CardPileCmd.Add(cardsToDraw, PileType.Hand);
        }

        await DownfallHook.AfterCustomDraw(player.Creature.CombatState, ctx, player, pileType, result);
        return result;
    }

    public static async Task<IReadOnlyList<CardPileAddResult>> DrawFromCustomPile(PlayerChoiceContext ctx,
        Player player, PileType pileType, int amount)
    {
        var result = new List<CardPileAddResult>();
        for (var i = 0; i < amount; i++) result.Add(await DrawFromCustomPile(ctx, player, pileType));
        return result;
    }
    
    public static async Task SelectCardToMovePiles(PlayerChoiceContext ctx, CardModel card,PileType fromPile, PileType toPile)
    {
        var cards = fromPile.GetPile(card.Owner).Cards.ToList();
        if (cards.Count == 0) return;
        var want = card.DynamicVars.Cards.IntValue;
        IEnumerable<CardModel> newCard;
        if (cards.Count <= want)
        {
            newCard = cards;
        }
        else
        {
            var prefs = new CardSelectorPrefs(card.SelectionScreenPrompt, want, want);
            newCard = await CardSelectCmd.FromSimpleGrid(ctx, cards, card.Owner, prefs);
        }
        await CardPileCmd.Add(newCard, toPile);
    }
}