using BaseLib.Patches.Content;
using Downfall.DownfallCode.Events;
using Downfall.DownfallCode.Utils;
using Godot;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace Downfall.DownfallCode.Commands;

public class DownfallCardCmd
{
    public static async Task<T> GiveCard<T>(Player player,
        PileType pileType,
        CardPilePosition position = CardPilePosition.Bottom,
        bool upgraded = false,
        float animationTime = 0.6f,
        CardPreviewStyle animationStyle = CardPreviewStyle.HorizontalLayout,
        bool skipAnimation = false,
        Action<T>? action = null,
        Player? creator = null) where T : CardModel
    {
        creator ??= player;
        var card = (T)player.Creature.CombatState!.CreateCard(ModelDb.Card<T>(), player);
        if (upgraded) card.UpgradeInternal();
        action?.Invoke(card);
        var result = await CardPileCmd.AddGeneratedCardToCombat(card, pileType, creator, position);
        if (result.success && !skipAnimation && pileType != PileType.Hand)
            CardCmd.PreviewCardPileAdd(result, animationTime, animationStyle);
        return (T)result.cardAdded;
    }

    public static async Task<IEnumerable<T>> GiveCards<T>(Player player,
        PileType pileType,
        decimal count,
        CardPilePosition position = CardPilePosition.Bottom,
        bool upgraded = false,
        float animationTime = 0.6f,
        CardPreviewStyle animationStyle = CardPreviewStyle.HorizontalLayout,
        bool skipAnimation = false,
        Action<T>? action = null,
        Player? creator = null) where T : CardModel
    {
        creator ??= player;
        if (count <= 0) return [];
        var cardInstances = new List<CardModel>();
        var model = ModelDb.Card<T>();
        for (var i = 0; i < count; i++)
        {
            var card = (T)player.Creature.CombatState!.CreateCard(model, player);
            if (upgraded) card.UpgradeInternal();
            action?.Invoke(card);
            cardInstances.Add(card);
        }

        var result = await CardPileCmd.AddGeneratedCardsToCombat(cardInstances, pileType, creator, position);
        if (!skipAnimation && pileType != PileType.Hand)
            CardCmd.PreviewCardPileAdd(result, animationTime, animationStyle);
        return result.Select(e => (T)e.cardAdded);
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


    /// <summary>
    ///     Select from given cards with count manually specified.
    /// </summary>
    public static Task<IEnumerable<CardModel>> SelectFromCards(PlayerChoiceContext ctx,
        IReadOnlyList<CardModel> cards, LocString prompt, int count, CardModel cardSource,
        bool optional = false)
        => CardSelectCmd.FromSimpleGrid(ctx, cards, cardSource.Owner, Prefs(prompt, count, optional));

    /// <summary>
    ///     Select from given cards with count determined by <c>DynamicVars.Cards</c> or a default value of 1.
    /// </summary>
    public static Task<IEnumerable<CardModel>> SelectFromCards(PlayerChoiceContext ctx,
        IReadOnlyList<CardModel> cards, LocString prompt, CardModel cardSource,
        bool optional = false)
        => SelectFromCards(ctx, cards, prompt, GetCardCount(cardSource), cardSource, optional);

    public static Task<IEnumerable<CardModel>> SelectFromCombatPile(PlayerChoiceContext ctx,
        CardPile pile, LocString prompt, int count, CardModel cardSource, Func<CardModel, bool>? filter = null,
        bool optional = false)
        => CardSelectCmd.FromCombatPile(ctx, pile, cardSource.Owner, Prefs(prompt, count, optional), filter);

    public static Task<IEnumerable<CardModel>> SelectFromCombatPile(PlayerChoiceContext ctx,
        CardPile pile, LocString prompt, CardModel cardSource, Func<CardModel, bool>? filter = null,
        bool optional = false)
        => SelectFromCombatPile(ctx, pile, prompt, GetCardCount(cardSource), cardSource, filter, optional);

    /// <summary>
    ///     Select cards from hand with count manually specified.
    /// </summary>
    public static Task<IEnumerable<CardModel>> SelectFromHand(PlayerChoiceContext ctx, LocString prompt,
        int count, AbstractModel source,
        Func<CardModel, bool>? filter = null, bool optional = false)
        => CardSelectCmd.FromHand(ctx, source.GetCreature().Player!, Prefs(prompt, count, optional), filter, source);


    /// <summary>
    ///     Select cards from hand with count determined by <c>DynamicVars.Cards</c> or a default value of 1.
    /// </summary>
    public static Task<IEnumerable<CardModel>> SelectFromHand(PlayerChoiceContext ctx, LocString prompt,
        CardModel cardSource,
        Func<CardModel, bool>? filter = null, bool optional = false)
        => SelectFromHand(ctx, prompt, GetCardCount(cardSource), cardSource, filter, optional);

    /// <summary>
    ///     Select cards from hand with count determined by <c>Amount</c>.
    /// </summary>
    public static Task<IEnumerable<CardModel>> SelectFromHand(PlayerChoiceContext ctx, LocString prompt,
        PowerModel powerSource,
        Func<CardModel, bool>? filter = null, bool optional = false)
        => SelectFromHand(ctx, prompt, powerSource.Amount, powerSource, filter, optional);

    public static void ForceUpgrade(CardModel card, int upgrade = 1)
    {
        ForceUpgradeHelper.ForceUpgrade(card, upgrade);
    }


    public static async Task AddGeneratedCardToCombatAtIndex(
        CardModel card, CardPile cardPile, int index, Player? creator)
    {
        if (!CombatManager.Instance.IsInProgress) return;
        if (card.Pile != null)
            throw new InvalidOperationException("You are not allowed to generate cards that already have a pile");
        if (!cardPile.Type.IsCombatPile())
            throw new InvalidOperationException("Generated cards must go to a combat pile");

        var combatState = card.Owner.Creature.CombatState;
        if (combatState == null) return;

        CombatManager.Instance.History.CardGenerated(combatState, card, creator);

        cardPile.AddInternal(card, index);
        cardPile.InvokeCardAddFinished();

        await Hook.AfterCardEnteredCombat(combatState, card);

        await Hook.AfterCardChangedPiles(
            card.Owner.RunState, combatState, card, PileType.None, null);

        await Hook.AfterCardGeneratedForCombat(combatState, card, creator);

        CardCmd.PreviewCardPileAdd(
            new CardPileAddResult { cardAdded = card, success = true, oldPile = null, modifyingModels = null },
            0.6f);
    }

    private static int GetCardCount(CardModel cardSource) =>
        cardSource.DynamicVars.ContainsKey("Cards") ? cardSource.DynamicVars.Cards.IntValue : 1;

    private static CardSelectorPrefs Prefs(LocString prompt, int count, bool optional) =>
        new(prompt, optional ? 0 : count, count);


    public static async Task AnimateCardFromRewardScreen(PileType pile, CardModel card, Player player)
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
        var fly = NCardFlyVfx.Create(node, pile, true, player.Character.TrailPath);
        trailContainer.AddChildSafely(fly);
        if (fly != null)
            await fly.ToSignal(fly, Node.SignalName.TreeExited);
    }


    public static async Task<CardPileAddResult> DrawFromCustomPile(PlayerChoiceContext ctx, Player player,
        PileType pileType)
    {
        if (player.Creature.CombatState == null) return default;
        var pile = CustomPiles.GetCustomPile(player.PlayerCombatState, pileType);
        CardPileAddResult result;
        if (pile == null || pile.Cards.Count == 0)
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
}