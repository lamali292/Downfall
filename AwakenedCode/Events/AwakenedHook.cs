using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Awakened.AwakenedCode.Events;

/// <summary>
/// Dispatch points for Awakened's custom combat hooks. Each method fans a game event out to every
/// <see cref="IOnDrained"/>/<see cref="IOnChant"/>/<see cref="IOnAwaken"/>/<see cref="IModifyManaburnDamage"/>/
/// <see cref="IModifyBaseSpells"/> listener in the combat (powers, relics, cards, ...) via
/// <c>HookUtils</c>. Call sites raise the event; listeners implement the matching interface to react to it.
/// </summary>
public static class AwakenedHook
{
    /// <summary>
    /// Raised whenever a player loses energy (<see cref="PlayerCmdLoseEnergyPatch"/> patches every energy
    /// loss; <c>Spew</c> also raises it directly for its own energy spend). Lets listeners like
    /// <c>ManaburnPower</c> react to a drain regardless of what caused it.
    /// </summary>
    /// <param name="cs">Combat the drain happened in; hook is a no-op if null.</param>
    /// <param name="ctx">Choice context to run any follow-up player choices/animations through.</param>
    /// <param name="player">The player who lost energy.</param>
    /// <param name="amount">How much energy was lost.</param>
    public static Task OnDrained(ICombatState? cs, PlayerChoiceContext ctx, Player player, int amount)
    {
        return HookUtils.Dispatch<IOnDrained>(cs, m => m.OnDrained(ctx, player, amount));
    }

    /// <summary>
    /// Raised from <see cref="AwakenedCmd.Chant"/> after a card's chant effect has played and its
    /// <c>HasChanted</c> flag has been set. Lets listeners react to (or trigger extra) chants, e.g.
    /// <c>RisingChorusPower</c> doubling the turn's first chant, or <c>Caw</c> scaling off other Caws.
    /// </summary>
    /// <param name="cs">Combat the chant happened in.</param>
    /// <param name="ctx">Choice context to run the reaction through.</param>
    /// <param name="card">The card whose chant effect just played.</param>
    /// <param name="cardPlay">The play (target, resources spent, ...) the chant is attached to.</param>
    /// <param name="firstTime">
    /// True only the very first time this specific card instance has ever chanted in its lifetime
    /// (i.e. <c>HasChanted</c> was false before this call). Stays false on every later chant of the
    /// same card, even in a new turn - it does not mean "first chant this turn". Used for one-off
    /// per-card flavor (banter/SFX, tooltip wording), not for turn-scoped logic.
    /// </param>
    /// <param name="isFirstChantInSeries">
    /// True for the "genuine" chant that came from actually playing the card; false when this chant
    /// was itself triggered as a bonus activation by another effect (e.g. Rising Chorus recursing
    /// into <see cref="AwakenedCmd.Chant"/> a second time). Use this to tell a real chant apart from
    /// an echo of one - e.g. to avoid re-triggering off a chant that is already a bonus trigger.
    /// </param>
    public static Task OnCardChanted(ICombatState cs, PlayerChoiceContext ctx, CardModel card, CardPlay cardPlay,
        bool firstTime, bool isFirstChantInSeries)
    {
        return HookUtils.Dispatch<IOnChant>(cs, ctx,
            m => m.OnCardChanted(card, ctx, cardPlay, firstTime, isFirstChantInSeries));
    }

    /// <summary>
    /// Raised from <see cref="AwakenedCmd.Awaken"/> once a player crosses the Awaken meter threshold
    /// and has actually been marked Awakened (fires once per combat, after the cast visuals play).
    /// </summary>
    /// <param name="cs">Combat the player Awakened in.</param>
    /// <param name="ctx">Choice context to run the reaction through.</param>
    /// <param name="player">The player who Awakened.</param>
    public static Task OnAwaken(ICombatState cs, PlayerChoiceContext ctx, Player player)
    {
        return HookUtils.Dispatch<IOnAwaken>(cs, ctx, m => m.OnAwaken(ctx, player));
    }

    /// <summary>
    /// Lets listeners modify a pending Manaburn hit before it lands (called from
    /// <c>ManaburnPower</c> right after a drain, before the HP loss is applied). Pair this with
    /// <see cref="AfterModifyingManaburnDamage"/> - call this first to get the final amount and the
    /// list of modifiers that touched it, then pass that list to the after-hook.
    /// </summary>
    /// <param name="cs">Combat the Manaburn hit is happening in.</param>
    /// <param name="original">The unmodified Manaburn damage (power amount * drained amount).</param>
    /// <param name="player">The player about to take the Manaburn hit.</param>
    /// <param name="modifiers">
    /// Out: every <see cref="IModifyManaburnDamage"/> listener that actually changed the amount, in
    /// application order - pass this straight into <see cref="AfterModifyingManaburnDamage"/>.
    /// </param>
    /// <returns>The damage amount after all modifiers have been applied.</returns>
    public static decimal ModifyManaburnDamage(ICombatState cs, decimal original, Player player,
        out IEnumerable<IModifyManaburnDamage> modifiers)
    {
        return HookUtils.Modify(cs, original, (e, amount) => e.ModifyManaburnDamage(amount, original, player),
            out modifiers);
    }

    /// <summary>
    /// Notifies each modifier from a prior <see cref="ModifyManaburnDamage"/> call after the final
    /// amount has been applied, so they can play follow-up effects/visuals (e.g. a relic that reacts
    /// to having reduced a Manaburn hit).
    /// </summary>
    /// <param name="cs">Combat the Manaburn hit happened in.</param>
    /// <param name="ctx">Choice context to run any follow-up effects through.</param>
    /// <param name="player">The player who took the Manaburn hit.</param>
    /// <param name="modifiers">The modifiers returned by the matching <see cref="ModifyManaburnDamage"/> call.</param>
    public static Task AfterModifyingManaburnDamage(ICombatState cs, PlayerChoiceContext ctx, Player player,
        IEnumerable<IModifyManaburnDamage> modifiers)
    {
        return HookUtils.AfterModifying(cs, modifiers, e => e.AfterModifyingManaburnDamage(ctx, player));
    }

    /// <summary>
    /// Lets listeners add to (or otherwise change) the pool of spell types the Spellbook can draw
    /// from. Called from <c>AwakenedPile</c> whenever the base spell list is needed, e.g. on refresh.
    /// </summary>
    /// <param name="cs">Combat the spellbook belongs to.</param>
    /// <param name="owner">The player whose spellbook is being built.</param>
    /// <param name="original">The base spell card list before any listener has touched it.</param>
    /// <returns>The spell card list after every listener has had a chance to modify it.</returns>
    public static IReadOnlyList<CardModel> ModifyBaseSpells(ICombatState cs, Player owner,
        IReadOnlyList<CardModel> original)
    {
        return HookUtils.Aggregate<IModifyBaseSpells, IReadOnlyList<CardModel>>(cs, original,
            (e, types) => e.ModifyBaseSpells(owner, types));
    }
}