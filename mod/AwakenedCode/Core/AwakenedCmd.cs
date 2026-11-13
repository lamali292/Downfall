using Awakened.AwakenedCode.Events;
using Awakened.AwakenedCode.Piles;
using Awakened.AwakenedCode.Powers;
using Awakened.AwakenedCode.Vfx;
using BaseLib.Utils;
using Downfall.DownfallCode.Core;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Awakened.AwakenedCode.Core;

public static class AwakenedCmd
{
    public static AwakenedPile GetSpellbook(Player player)
    {
        return (AwakenedPile)AwakenedPile.Spellbook.GetPile(player);
    }

    
    private static readonly PlayerField<bool> SpellbookInitialized = new(() => false);
    public static void InitSpellbook(Player player)
    {
        if (SpellbookInitialized.Get(player)) return;
        SpellbookInitialized.Set(player, true);
        GetSpellbook(player).Refresh(player);
    }


    private static void RefreshSpellbook(Player player)
    {
        GetSpellbook(player).Refresh(player);
    }

    public static async Task Awaken(Player player, PlayerChoiceContext ctx)
    {
        if (!AwakenedModel.MarkAwakened(player)) return;

        var creatureNode = NCombatRoom.Instance?.GetCreatureNode(player.Creature);
        if (creatureNode?.Visuals is not NAwakenedCreatureVisuals awakenedVisuals) return;
        awakenedVisuals.SetParticles(true);
        await CreatureCmd.TriggerAnim(player.Creature, "Cast", player.Character.CastAnimDelay);
        await AwakenedHook.OnAwaken(player.Creature.CombatState!, ctx, player);
    }

    private static bool CanConjure(Player player)
    {
        return !player.Creature.Powers.Any(p => p is BurnoutPower);
    }

    public static async Task<CardModel?> Conjure(
        Player player)
    {
        if (!CanConjure(player)) return null;
        InitSpellbook(player);
        var spellbook = GetSpellbook(player);
        while (spellbook.NextSpell == null) spellbook.SetNextSpell(player);
        var spell = spellbook.NextSpell;
        if (spell == null) return null;
        return await ConjureSpell(player, spell, spellbook);
    }

    public static async Task<CardModel?> ConjureSelected(
        Player player,
        CardModel sourceCard,
        CardModel selectedSpell)
    {
        if (!CanConjure(player)) return null;
        InitSpellbook(player);
        var spellbook = GetSpellbook(player);
        if (!spellbook.Cards.Contains(selectedSpell)) return null;
        return await ConjureSpell(player, selectedSpell, spellbook);
    }

    private static async Task<CardModel?> ConjureSpell(
        Player player,
        CardModel spell,
        AwakenedPile spellbook)
    {
  
        await Cmd.Wait(0.1f);
        spellbook.RemoveInternal(spell);

        await CardPileCmd.AddGeneratedCardToCombat(
            spell,
            PileType.Hand,
            player);

        if (spellbook.Cards.Count == 0) RefreshSpellbook(player);

        spellbook.SetNextSpell(player);

        if (LocalContext.IsMe(player))
            Callable.From(() => NSpellbookButton.RevealFor(player)).CallDeferred();
        //AwakenedDisplay.RefreshSpellDisplays(player);
        return spell;
    }
}