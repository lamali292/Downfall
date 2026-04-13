using Downfall.Code.Displays;
using Downfall.Code.Interfaces;
using Downfall.Code.Piles;
using Downfall.Code.Powers.Awakened;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Random;

namespace Downfall.Code.Commands;

public static class AwakenedCmd
{
    public static AwakenedPile? GetSpellbook(Player player)
    {
        return AwakenedPile.Spellbook.GetPile(player) as AwakenedPile;
    }


    
    public static bool IsAwakened(Creature creature)
    {
        return creature.Powers.Any(p => p is AwakenedFormPower);
    }

    public static async Task Awaken(Player player, PlayerChoiceContext ctx)
    {
        if (IsAwakened(player.Creature)) return;

        // Todo: Update all spells everywhere to be upgraded
        await PowerCmd.Apply<AwakenedFormPower>(player.Creature, 1, player.Creature, null);
        var spellbook = GetSpellbook(player);
        if (spellbook != null)
        {
            foreach (var card in spellbook.Cards.Where(c => c.IsUpgradable))
            {
                card.UpgradeInternal();
                card.FinalizeUpgradeInternal();
            }

            spellbook.UpgradeOnAdd = true;
            AwakenedDisplay.Refresh(player);
        }

        foreach (var model in player.Creature.CombatState!.IterateHookListeners().OfType<IOnAwaken>())
            await model.OnAwaken(ctx, player);
    }

    public static async Task Chant(PlayerChoiceContext ctx, CardModel card, CardPlay cardPlay)
    {
        if (card is not IChantable chantable) return;
        chantable.HasChanted = true;
        await chantable.OnChant(ctx, cardPlay);
        if (cardPlay.Card.CombatState == null) return;
        var listeners = cardPlay.Card.CombatState.IterateHookListeners().OfType<IOnChant>();
        foreach (var listener in listeners) await listener.OnCardChanted(card, ctx, cardPlay);
    }
    
    public static bool CanConjure(Player player) 
        => !player.Creature.Powers.Any(p => p is BurnoutPower);
    
    public static async Task<CardModel?> Conjure(
        Player player,
        CombatState state)
    {
        if (!CanConjure(player)) return null;
        var spellbook = GetSpellbook(player);
        var rng = state.RunState.Rng.CombatCardSelection;
        if (spellbook == null) return null;

        var spell = spellbook.NextSpell ?? (spellbook.Cards.Count > 0 ? spellbook.Cards[0] : null);
        if (spell == null) return null;

        return await ConjureSpell(player, spell, spellbook, rng);
    }

    public static async Task<CardModel?> ConjureSelected(
        Player player,
        CardModel sourceCard,
        CardModel selectedSpell)
    {
        if (!CanConjure(player)) return null;
        var spellbook = GetSpellbook(player);
        var rng = sourceCard.CombatState!.RunState.Rng.CombatCardSelection;
        if (spellbook == null) return null;

        if (!spellbook.Cards.Contains(selectedSpell)) return null;
        return await ConjureSpell(player, selectedSpell, spellbook, rng);
    }

    private static async Task<CardModel?> ConjureSpell(
        Player player,
        CardModel spell,
        AwakenedPile spellbook,
        Rng rng) // replace 'object' with the actual RNG type used in your codebase
    {
        spellbook.RemoveInternal(spell);
        spellbook.SetNextSpell(rng);
        await CardPileCmd.Add(spell, PileType.Hand);

        if (spellbook.Cards.Count == 0) spellbook.Refresh(player);
        AwakenedDisplay.Refresh(player);
        return spell;
    }
}