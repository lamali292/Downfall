using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Awakened.AwakenedCode.Events;

/// <summary>
/// Dispatch points for Awakened's custom combat hooks. Each method fans a game event out to every
/// <see cref="IOnDrained"/>/<see cref="IModifyChantRepeatCount"/>/<see cref="IOnAwaken"/>/
/// <see cref="IModifyManaburnDamage"/>/<see cref="IModifyBaseSpells"/> listener in the combat (powers,
/// relics, cards, ...) via <c>HookUtils</c>. Call sites raise the event; listeners implement the
/// matching interface to react to it.
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
    /// Lets listeners (e.g. <c>RisingChorusPower</c>) make a card chant extra times beyond the one that
    /// just happened. Queried once per <see cref="ChantCmd.Chant"/> call, before any of that call's
    /// activations run - the resulting extra activations never query this hook again, so a listener
    /// can't recurse into itself through it.
    /// </summary>
    /// <param name="cs">Combat the chant happened in.</param>
    /// <param name="card">The card that just chanted.</param>
    /// <param name="cardPlay">The play the chant is attached to.</param>
    /// <param name="original">The repeat count before any listener has touched it (normally 1).</param>
    /// <param name="modifiers">
    /// Out: every <see cref="IModifyChantRepeatCount"/> listener that actually changed the count - pass
    /// this straight into <see cref="AfterModifyingChantRepeatCount"/>.
    /// </param>
    /// <returns>How many times the card should chant in total.</returns>
    public static int ModifyChantRepeatCount(ICombatState cs, CardModel card, CardPlay cardPlay, int original,
        out IEnumerable<IModifyChantRepeatCount> modifiers)
    {
        return HookUtils.Modify<IModifyChantRepeatCount, int>(cs, original,
            (e, count) => e.ModifyChantRepeatCount(card, cardPlay, count), out modifiers);
    }

    /// <summary>
    /// Notifies each modifier from a prior <see cref="ModifyChantRepeatCount"/> call (e.g. so
    /// <c>RisingChorusPower</c> can refresh its displayed remaining-uses amount) once the repeat count
    /// is final, before any chant activation runs.
    /// </summary>
    /// <param name="cs">Combat the chant happened in.</param>
    /// <param name="card">The card that just chanted.</param>
    /// <param name="cardPlay">The play the chant is attached to.</param>
    /// <param name="modifiers">The modifiers returned by the matching <see cref="ModifyChantRepeatCount"/> call.</param>
    public static Task AfterModifyingChantRepeatCount(ICombatState cs, CardModel card, CardPlay cardPlay,
        IEnumerable<IModifyChantRepeatCount> modifiers)
    {
        return HookUtils.AfterModifying(cs, modifiers, e => e.AfterModifyingChantRepeatCount(card, cardPlay));
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