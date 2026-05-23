using Downfall.DownfallCode.Events;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Awakened.AwakenedCode.Events;

public static class AwakenedHook
{
    public static Task OnDrained(ICombatState cs, PlayerChoiceContext ctx, Player player, int amount)
    {
        return DownfallHook.Dispatch<IOnDrained>(cs, ctx, m => m.OnDrained(ctx, player, amount));
    }

    public static Task OnCardChanted(ICombatState cs, PlayerChoiceContext ctx, CardModel card, CardPlay cardPlay,
        bool firstTime)
    {
        return DownfallHook.Dispatch<IOnChant>(cs, ctx, m => m.OnCardChanted(card, ctx, cardPlay, firstTime));
    }

    public static Task OnAwaken(ICombatState cs, PlayerChoiceContext ctx, Player player)
    {
        return DownfallHook.Dispatch<IOnAwaken>(cs, ctx, m => m.OnAwaken(ctx, player));
    }


    public static decimal ModifyManaburnDamage(ICombatState cs, decimal original, Player player,
        out IEnumerable<IModifyManaburnDamage> modifiers)
    {
        return DownfallHook.Modify(cs, original, (e, amount) => e.ModifyManaburnDamage(amount, original, player),
            out modifiers);
    }

    public static Task AfterModifyingManaburnDamage(ICombatState cs, PlayerChoiceContext ctx, Player player,
        IEnumerable<IModifyManaburnDamage> modifiers)
    {
        return DownfallHook.AfterModifying(cs, modifiers, e => e.AfterModifyingManaburnDamage(ctx, player));
    }


    public static IReadOnlyList<CardModel> ModifyBaseSpells(ICombatState cs, Player owner,
        IReadOnlyList<CardModel> original)
    {
        return DownfallHook.Aggregate<IModifyBaseSpells, IReadOnlyList<CardModel>>(cs, original,
            (e, types) => e.ModifyBaseSpells(owner, types));
    }
}