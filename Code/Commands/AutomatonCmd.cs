using BaseLib.Patches.Content;
using Downfall.Code.Cards.Automaton.Rare;
using Downfall.Code.Cards.Automaton.Token;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Cards.Piles;
using Downfall.Code.Interfaces;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using NSequenceDisplay = Downfall.Code.Cards.Vfx.NSequenceDisplay;

namespace Downfall.Code.Commands;

public record CompileContext(int SlotIndex, int TotalCards)
{
    public bool IsFirst => SlotIndex == 0;
    public bool IsLast => SlotIndex == TotalCards - 1;
    public bool IsMiddle => !IsFirst && !IsLast;
}

public static class AutomatonCmd
{
    private static readonly Dictionary<Creature, NSequenceDisplay> _displays = new();

    static AutomatonCmd()
    {
        CombatManager.Instance.CombatEnded += _ =>
        {
            foreach (var d in _displays.Values)
                d.QueueFree();
            _displays.Clear();
        };
    }

    public static int GetSequenceCount(Creature creature)
    {
        return GetEncodePile(creature)?.Cards.Count ?? 0;
    }

    public static IReadOnlyList<CardModel> GetSequence(Creature creature)
    {
        return GetEncodePile(creature)?.Cards ?? [];
    }

    public static AutomatonPile? GetEncodePile(Creature creature)
    {
        return creature.Player == null
            ? null
            : CustomPiles.GetCustomPile(creature.Player.PlayerCombatState, AutomatonPile.FunctionSequence) as AutomatonPile;
    }

    public static int GetMax(Creature creature)
    {
        return 3;
    }

    public static async Task EncodeCard(
        CardModel card,
        PlayerChoiceContext ctx,
        CardPlay cardPlay)
    {
        var creature = cardPlay.Card.Owner.Creature;
        var pile = GetEncodePile(creature);
        if (pile == null) return;

        var display = _displays.GetValueOrDefault(creature);
        var slotIndex = pile.Cards.Count;
        var targetPos = display?.GetSlotGlobalPosition(slotIndex);

        var cardNode = NCard.FindOnTable(card);
        var isMe = LocalContext.IsMe(creature);
        await CardPileCmd.Add(card, pile, skipVisuals: isMe);

        // Animate the card from hand to its sequence slot
        if (cardNode != null && targetPos.HasValue && isMe)
        {
            var hand = NCombatRoom.Instance?.Ui.Hand;
            if (hand?.GetCardHolder(card) != null)
                hand.Remove(card);
            var vfx = NCombatRoom.Instance?.CombatVfxContainer;
            if (vfx != null)
            {
                var gp = cardNode.GlobalPosition;
                cardNode.Reparent(vfx);
                cardNode.GlobalPosition = gp;
            }

            var tween = cardNode.CreateTween().SetParallel();
            tween.TweenProperty(cardNode, "global_position", targetPos.Value, 0.3f)
                .SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Expo);
            tween.TweenProperty(cardNode, "scale", Vector2.One * NSequenceDisplay.SequencedCardScale, 0.3f)
                .SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Expo);
            await cardNode.ToSignal(tween, Tween.SignalName.Finished);
            cardNode.QueueFree();
        }

        if (LocalContext.IsMe(creature))
            display?.Refresh();

        foreach (var model in creature.CombatState!.IterateHookListeners()
                     .OfType<IOnEncode>())
            await model.OnCardEncoded(ctx, card, cardPlay);


        if (pile.Cards.Count >= GetMax(creature))
            await CompileFunctionCard(creature, ctx, cardPlay);
    }

    public static async Task CompileFunctionCard(
        Creature creature,
        PlayerChoiceContext ctx,
        CardPlay cardPlay)
    {
        var pile = GetEncodePile(creature);
        if (pile == null) return;
        await Cmd.Wait(0.3f);
        var snapshot = pile.Cards.OfType<AutomatonCardModel>().ToList();
        pile.Clear(true);

        if (LocalContext.IsMe(creature))
            _displays.GetValueOrDefault(creature)?.Refresh();

        FunctionCard functionCard;

        if (snapshot.Any(c => c is FullRelease))
            functionCard = creature.CombatState!.CreateCard<FunctionPowerCard>(cardPlay.Card.Owner);
        else if (snapshot.Any(c => c.TargetType == TargetType.AnyEnemy || c.Type == CardType.Attack))
            functionCard = creature.CombatState!.CreateCard<FunctionAttackCard>(cardPlay.Card.Owner);
        else
            functionCard = creature.CombatState!.CreateCard<FunctionSkillCard>(cardPlay.Card.Owner);

        functionCard.SetSourceCards(snapshot);
        for (var i = 0; i < snapshot.Count; i++)
        {
            var card = snapshot[i];
            var compileContext = new CompileContext(i, snapshot.Count);

            switch (card)
            {
                case ICompilableError compileErrorCard when !card.SuppressCompileError:
                    await compileErrorCard.OnCompileError(ctx, functionCard, cardPlay, compileContext, true);
                    break;
                case ICompilable compileCard:
                    await compileCard.OnCompile(ctx, functionCard, cardPlay, compileContext, true);
                    break;
            }
        }

        foreach (var model in creature.CombatState!.IterateHookListeners()
                     .OfType<IOnCompile>())
            await model.OnCompile(ctx, snapshot, functionCard, cardPlay);


        var result = await CardPileCmd.AddGeneratedCardToCombat(functionCard, PileType.Hand, true);

        if (result.success)
            CardCmd.PreviewCardPileAdd(result, 0.7f);
    }

    public static void RefreshDisplay(Creature creature)
    {
        _displays.GetValueOrDefault(creature)?.Refresh();
    }

    public static async Task MoveFromSequenceToHand(CardModel card, Creature creature)
    {
        await CardPileCmd.Add(card, PileType.Hand);

        if (LocalContext.IsMe(creature))
            _displays.GetValueOrDefault(creature)?.Refresh();
    }

    public static async Task MoveFromSequenceToHand(IEnumerable<CardModel> cards, Creature creature)
    {
        await CardPileCmd.Add(cards, PileType.Hand);
        if (LocalContext.IsMe(creature))
            _displays.GetValueOrDefault(creature)?.Refresh();
    }

    public static void ClearSequence(Creature creature)
    {
        GetEncodePile(creature)?.Clear(true);
        if (!_displays.TryGetValue(creature, out var display)) return;
        display.QueueFree();
        _displays.Remove(creature);
    }

    public static void RegisterDisplay(Creature creature, NSequenceDisplay display)
    {
        if (_displays.TryGetValue(creature, out var old))
            old.QueueFree();
        _displays[creature] = display;
    }
}