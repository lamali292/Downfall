using Hermit.HermitCode.CustomEnums;
using Hermit.HermitCode.Events;
using Hermit.HermitCode.History;
using Hermit.HermitCode.Patches;
using Hermit.HermitCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Hermit.HermitCode.Core;

public static class HermitCmd
{

    public static Task Concentrate(PlayerChoiceContext ctx, AbstractModel model, int amount = 1)
    {
        return PowerCmd.Apply<ConcentrationPower>(ctx, model.Creature, amount, model.Creature, model as CardModel);
    }
    
    /// <summary>
    ///     The one Dead On question: is this card Dead On right now, for the play it is part of?
    ///     <para>
    ///         <c>IShouldTriggerDeadOn</c> sources are always re-asked live (their answer can change
    ///         between replay instances of the same card). Hand-position Dead On is read live while
    ///         the card is in hand, and from <see cref="DeadOnPatch" />'s pre-play snapshot once it
    ///         sits in the Play pile (hand position is gone by then but never changes during a play).
    ///     </para>
    /// </summary>
    public static bool IsDeadOn(CardModel card)
    {
        if (card.CombatState != null && HermitHook.ShouldTriggerDeadOn(card.CombatState, card))
            return true;
        return card.Pile?.Type switch
        {
            PileType.Hand => IsDeadOnInCurrentHandState(card),
            PileType.Play => WasThisPlayedDeadOn(card),
            _ => false
        };
    }
    
    public static bool IsAdjacentToCurse(CardModel card)
    {
        return card.Pile?.Type switch
        {
            PileType.Hand => IsAdjacentToCurseInCurrentHandState(card),
            PileType.Play => WasThisPlayedAdjacentToCurse(card),
            _ => false
        };
    }
    
    private static bool IsDeadOnInCurrentHandState(CardModel card)
    {
        return HandGeometry.IsCenter(card.Owner.Hand, card);
    }
    
    private static bool IsAdjacentToCurseInCurrentHandState(CardModel cardModel)
    {
        return HandGeometry.IsAdjacentToMatch(cardModel.Owner.Hand, cardModel, c => c.Type == CardType.Curse);
    }
    
    private static bool WasThisPlayedDeadOn(CardModel card)
    {
        return DeadOnPatch.StatusOf(card).IsCenter;
    }
    
    private static bool WasThisPlayedAdjacentToCurse(CardModel card)
    {
        return DeadOnPatch.StatusOf(card).IsAdjacentToCurse;
    }

    /// <summary>Reads the card's current hand position. Called once, by <see cref="DeadOnPatch" />, at play start.</summary>
    internal static PlayStartHandStatus CaptureHandStatus(CardModel card)
    {
        return new PlayStartHandStatus(IsDeadOnInCurrentHandState(card), IsAdjacentToCurseInCurrentHandState(card));
    }
    
    public static bool HasActiveDeadOnEffect(CardModel card)
    {
        return IsDeadOn(card) && HasDeadOn(card);
    }

    public static bool HasDeadOn(CardModel card)
    {
        return card.Keywords.Contains(HermitKeywords.DeadOn);
    }

    /// <summary>
    ///     Raw <c>IModifyDeadOnCount</c> hook query (e.g. <c>SnipePower</c>'s +1) - the same count
    ///     <see cref="DeadOn" /> uses to decide how many times to run its own effect loop. Doesn't
    ///     gate on whether the card is actually Dead On right now; callers that already established
    ///     that (<see cref="DeadOn" /> via <see cref="HasActiveDeadOnEffect" />, <c>DeadOnReplay</c>
    ///     via <see cref="IsDeadOn" />) call this instead of checking a specific power (Snipe) by
    ///     hand - keeps every Dead On multiplier source (present and future) in one place.
    /// </summary>
    public static int DeadOnMultiplier(CardModel card)
    {
        return HermitHook.ModifyDeadOnCount(card.CombatState!, 1, card, out _);
    }

    /// <summary>
    ///     The mutating counterpart to <see cref="DeadOnMultiplier" />: resolves the current count
    ///     AND fires <c>AfterModifyingDeadOnCount</c> so one-shot sources (Snipe) actually get
    ///     consumed. Call this at most once per real Dead On trigger - <see cref="DeadOn" /> is the
    ///     usual caller; <c>DeadOnReplay</c> calls it directly since the card it enchants never runs
    ///     through <see cref="DeadOn" /> itself.
    /// </summary>
    public static async Task<int> ConsumeDeadOnMultiplier(CardModel card)
    {
        var combatState = card.CombatState!;
        var modify = HermitHook.ModifyDeadOnCount(combatState, 1, card, out var modifiers);
        await HermitHook.AfterModifyingDeadOnCount(combatState, card, modifiers);
        return modify;
    }

    /// <summary>
    ///     Gate-and-run for a card's own Dead On effect: called by the card itself from
    ///     <c>OnPlayInternal</c> (or <c>OnTurnEndInHand</c> for <c>ImpendingDoom</c>), mirroring
    ///     how Chant/Overflow are triggered. No-op (besides history/hooks) for cards that only
    ///     care about Dead On via another hook (e.g. Headshot's damage multiplier) - pass no
    ///     <paramref name="effect" /> for those.
    /// </summary>
    public static async Task DeadOn(PlayerChoiceContext ctx, CardModel card, CardPlay? cardPlay = null,
        Func<Task>? effect = null)
    {
        if (!HasActiveDeadOnEffect(card)) return;

        var modify = await ConsumeDeadOnMultiplier(card);
        for (var i = 0; i < modify && effect != null; i++)
            await effect.Invoke();
        await RecordDeadOnTrigger(ctx, card, cardPlay);
    }

    /// <summary>
    ///     Records a <see cref="DeadOnEntry" /> and fires <see cref="IAfterDeadOnTrigger" /> - the
    ///     part of a Dead On trigger that other cards read back (Called Shot's "did my last play
    ///     trigger Dead On" check, Combo, ...). <see cref="DeadOn" /> calls this itself; <c>
    ///     DeadOnReplay</c> calls it directly from its own <c>OnPlay</c> override (once per replay
    ///     iteration, with that iteration's real <see cref="CardPlay" />) since the card it's
    ///     attached to never runs through <see cref="DeadOn" />.
    /// </summary>
    public static async Task RecordDeadOnTrigger(PlayerChoiceContext ctx, CardModel card, CardPlay? cardPlay)
    {
        var combatState = card.CombatState!;
        var entry = new DeadOnEntry(cardPlay, card.Owner.Creature, combatState.RoundNumber,
            card.Owner.Creature.Side, CombatManager.Instance.History, combatState.Players);
        CombatManager.Instance.History.Add(combatState, entry);
        await HermitHook.AfterDeadOnTrigger(combatState, ctx, card);
    }
}