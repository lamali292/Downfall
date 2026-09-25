using BaseLib.Commands;
using BaseLib.Patches.Content;
using Downfall.DownfallCode.Compatibility;
using Downfall.DownfallCode.Events;
using Downfall.DownfallCode.Utils;
using Godot;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Runs.History;
using MegaCrit.Sts2.Core.TestSupport;

namespace Downfall.DownfallCode.Commands;

/// <summary>
///     Creates cards and gets them into a player's possession: giving cards to a pile, generating
///     at a specific index, drawing from a custom pile, auto-playing off the draw pile, the
///     reward-screen fly-in animation, and finding cards from the unlocked pool. Player-facing
///     selection prompts live in <see cref="DownfallCardSelectionCmd" />; destroy/removal visuals
///     live in <see cref="CardRemovalCmd" />.
/// </summary>
public class DownfallCardCmd
{
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

    /// <summary>
    ///     Finds unlocked cards matching <paramref name="cond" />.
    ///     If the player's character is <typeparamref name="T" />, only that character's own
    ///     card pool is searched; otherwise every character pool is searched.
    /// </summary>
    /// <typeparam name="T">Character type that scopes the search to a single pool when the player matches it.</typeparam>
    /// <param name="player">The player whose unlock state, run constraints, and character determine which cards are searched.</param>
    /// <param name="cond">Predicate each card must satisfy to be included.</param>
    /// <param name="count">Maximum number of distinct combat-legal cards to return.</param>
    public static IEnumerable<CardModel> GetSpecificCards<T>(Player player, Func<CardModel, bool> cond, int count = 1)
        where T : CharacterModel
    {
        var constraint = player.RunState.CardMultiplayerConstraint;
        var cards = player.Character is T
            ? player.Character.CardPool.GetUnlockedCards(player.UnlockState, constraint)
            : ModelDb.AllCharacterCardPools
                .SelectMany(e => e.GetUnlockedCards(player.UnlockState, constraint));

        return CardFactory.GetDistinctForCombat(player, cards.Where(cond), count,
            player.RunState.Rng.CombatCardGeneration);
    }
    
    public static async Task<IEnumerable<CardModel>> MulitPileSelect(
        PlayerChoiceContext ctx,
        Player player,
        CardSelectorPrefs prefs,
        List<CardModel> cards,
        PileType[]? pileTypes = null)
    {
        if (!TestMode.IsOn)
            return await MultiPileCardSelect.Select(
                ctx,
                player,
                prefs,
                cards,
                pileTypes);
        if (CardSelectCmd.Selector == null)
            return [];

        return await CardSelectCmd.Selector.GetSelectedCards(
            cards,
            prefs.MinSelect,
            prefs.MaxSelect);

    }
    
    public static async Task<IEnumerable<CardModel>> MulitPileSelect(
        PlayerChoiceContext ctx,
        Player player,
        CardSelectorPrefs prefs,
        Func<CardModel, bool>? filter = null,
        params PileType[] pileTypes)
    {
        if (!TestMode.IsOn)
            return await MultiPileCardSelect.Select(
                ctx,
                player,
                prefs,
                filter,
                pileTypes);
        if (CardSelectCmd.Selector == null)
            return [];

        return await CardSelectCmd.Selector.GetSelectedCards(
            pileTypes.SelectMany(e => e.GetPile(player).Cards),
            prefs.MinSelect,
            prefs.MaxSelect);

    }


    public static async Task RemoveFromCombat(IEnumerable<CardModel> cards)
    {
        var list = cards.ToList();
        foreach (var card in list)
        {
            await PlayDestroyPreview(card, 0.1f);
        }
        await CardPileCmd.RemoveFromCombat(list, true);
    }
    
    
    private static async Task PlayDestroyPreview(CardModel card, float delay)
    {
        if (!LocalContext.IsMine(card)) return;
 
        if (delay > 0f)
            await Cmd.Wait(delay);
 
        var cardNode = NCard.Create(card);
        if (cardNode == null) return;
 
        var room = NCombatRoom.Instance;
        if (room == null) return;
 
        var container = room.Ui.MessyCardPreviewContainer;
        container.AddChildSafely(cardNode);
        cardNode.UpdateVisuals(PileType.None, CardPreviewMode.Normal);
 
        var tween = cardNode.CreateTween();
        tween.TweenProperty(cardNode, "scale", Vector2.One, 0.25)
            .From(Vector2.Zero)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Cubic);
 
        if (!TestMode.IsOn && CardRemoveVfxCompat.IsAvailable)
        {
            tween.TweenInterval(0.25);
            tween.TweenCallback(Callable.From(() =>
            {
                var vfx = CardRemoveVfxCompat.Create(cardNode);
                if (vfx != null)
                    container.AddChildSafely(vfx);
            }));
            tween.TweenInterval(CardRemoveVfxCompat.DeleteCardDelay);
        }
        tween.TweenCallback(Callable.From(cardNode.QueueFreeSafely));
    }
    
    /// <summary>
    /// Be very careful using this method, it does not provide ANY checks for whether the card should be enchanted so will crash if the card already has an enchantment.
    /// You should almost always use CardCmd.Enchant instead of this method.
    /// Valid use cases include enchanting status cards and curse cards which are normally excluded from but function normally with enchantments.
    /// </summary>
    public static T? ForceEnchant<T>(CardModel card, Decimal amount) where T : EnchantmentModel
    {
        return ForceEnchant(ModelDb.Enchantment<T>().ToMutable(), card, amount) as T;
    }
    
    /// <summary>
    /// Forcefully applies an enchantment to a card without checking if it is supposed to.
    /// </summary>
    /// <param name="enchantment"> An enchantment you must check is valid BEFORE calling this method.</param>
    /// <param name="card"> A card that normally wouldn't be allowed to get an enchantment (I.E Curses).</param>
    /// <param name="amount"> The amount of stacks of this enchantment.</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static EnchantmentModel? ForceEnchant(
        EnchantmentModel enchantment,
        CardModel card,
        Decimal amount)
    {
        enchantment.AssertMutable();
        if (card.Enchantment == null)
        {
            card.EnchantInternal(enchantment, amount);
            enchantment.ModifyCard();
        }
        else if (card.Enchantment.GetType() == enchantment.GetType())
        {
            card.Enchantment.Amount += (int)amount;
        }
        else
        {
            return null;
        }
        card.FinalizeUpgradeInternal();
        CardPile pile = card.Pile;
        if (pile != null && pile.Type == PileType.Deck)
            card.Owner.RunState.CurrentMapPointHistoryEntry?.GetEntry(card.Owner.NetId).CardsEnchanted.Add(new CardEnchantmentHistoryEntry(card, enchantment.Id));
        return card.Enchantment;
    }
}
