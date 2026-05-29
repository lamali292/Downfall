using Awakened.AwakenedCode.Cards.Uncommon;
using Awakened.AwakenedCode.Displays;
using Awakened.AwakenedCode.Events;
using Awakened.AwakenedCode.Interfaces;
using Awakened.AwakenedCode.Piles;
using Awakened.AwakenedCode.Powers;
using Awakened.AwakenedCode.Vfx;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Random;

namespace Awakened.AwakenedCode.Core;

public static class AwakenedCmd
{
    public static AwakenedPile GetSpellbookOrThrow(Player player)
    {
        return (AwakenedPile)AwakenedPile.Spellbook.GetPile(player);
    }


    public static async Task Awaken(Player player, PlayerChoiceContext ctx)
    {
        if (!AwakenedModel.MarkAwakened(player)) return;

        Callable.From(() =>
        {
            var creatureNode = NCombatRoom.Instance?.GetCreatureNode(player.Creature);
            if (creatureNode?.Visuals is not NAwakenedCreatureVisuals awakenedVisuals) return;
            awakenedVisuals.IsAwakened = true;
            awakenedVisuals.OnAnimationTrigger("Idle");
        }).CallDeferred();
        await AwakenedHook.OnAwaken(player.Creature.CombatState!, ctx, player);
    }

    public static async Task Chant(PlayerChoiceContext ctx, CardModel card, CardPlay cardPlay)
    {
        if (card is not IChantable chantable) return;
        var firstTime = !chantable.HasChanted;
        if (firstTime && card is not Caw)
        {
            // TODO : change voice lines
            TalkCmd.Play(new LocString("monsters", "DAMP_CULTIST.moves.INCANTATION.banter"), card.Owner.Creature,
                VfxColor.Blue);
            SfxCmd.Play("event:/sfx/enemy/enemy_attacks/cultists/cultists_buff_damp");
        }

        chantable.HasChanted = true;
        await chantable.PlayChantEffect(ctx, cardPlay);
        await AwakenedHook.OnCardChanted(card.CombatState!, ctx, card, cardPlay, firstTime);
    }

    public static bool CanConjure(Player player)
    {
        return !player.Creature.Powers.Any(p => p is BurnoutPower);
    }

    public static async Task<CardModel?> Conjure(
        Player player,
        ICombatState state)
    {
        if (!CanConjure(player)) return null;
        var spellbook = AwakenedModel.GetOrInitSpellbook(player);
        var rng = state.RunState.Rng.CombatCardSelection;

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
        var spellbook = AwakenedModel.GetOrInitSpellbook(player);
        var rng = sourceCard.CombatState!.RunState.Rng.CombatCardSelection;

        if (!spellbook.Cards.Contains(selectedSpell)) return null;
        return await ConjureSpell(player, selectedSpell, spellbook, rng);
    }

    private static async Task<CardModel?> ConjureSpell(
        Player player,
        CardModel spell,
        AwakenedPile spellbook,
        Rng rng)
    {
        spellbook.RemoveInternal(spell);
        spellbook.SetNextSpell(rng);
        await CardPileCmd.Add(spell, PileType.Hand);

        if (spellbook.Cards.Count == 0) spellbook.Refresh(player);
        AwakenedDisplay.Refresh(player);
        return spell;
    }
}
