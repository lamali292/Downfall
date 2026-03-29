using Downfall.Code.Cards.Piles;
using Downfall.Code.Displays;
using Downfall.Code.Interfaces;
using Downfall.Code.Powers.Awakened;
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
    
    public static async Task<CardModel?> Conjure(
        Player player,
        CardModel card)
    {
        var spellbook = GetSpellbook(player);
        var rng = card.CombatState!.RunState.Rng.CombatCardSelection;
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
        Rng rng)          // replace 'object' with the actual RNG type used in your codebase
    {
        spellbook.RemoveInternal(spell);
        spellbook.SetNextSpell(rng);
        await CardPileCmd.Add(spell, PileType.Hand);

        if (spellbook.Cards.Count == 0) spellbook.Refresh(player);
        AwakenedDisplay.Refresh(player);
        return spell;
    }
}