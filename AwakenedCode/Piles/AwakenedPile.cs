using Awakened.AwakenedCode.Cards.Token;
using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Events;
using BaseLib.Abstracts;
using BaseLib.Patches.Content;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Random;

namespace Awakened.AwakenedCode.Piles;

public class AwakenedPile() : CustomPile(Spellbook)
{
    [CustomEnum] public static PileType Spellbook;

    private readonly List<CardModel> _dynamicTypes = [];

    public CardModel? NextSpell { get; private set; }


    public void AddPersistentType(CardModel type)
    {
        _dynamicTypes.Add(type.CanonicalInstance);
    }

    public override bool CardShouldBeVisible(CardModel card)
    {
        return false;
    }


    public override Vector2 GetTargetPosition(CardModel model, Vector2 size)
    {
        var creatureNode = NCombatRoom.Instance?.GetCreatureNode(model.Owner.Creature);
        return creatureNode?.GlobalPosition ?? Vector2.Zero;
    }


    public void SetNextSpell(Rng rng)
    {
        var available = Cards.Where(c => c != NextSpell).ToList();
        NextSpell = available.Count > 0
            ? rng.NextItem(available)
            : Cards.Count > 0
                ? Cards[0]
                : null;
    }

    public void Refresh(Player owner)
    {
        var state = owner.Creature.CombatState;
        if (state == null) return;

        var rng = state.RunState.Rng.CombatCardSelection;

        foreach (var card in Cards.ToList())
            card.RemoveFromState();

        AddBaseSpells(owner, state);

        foreach (var type in _dynamicTypes) CreateAndAddSpell(owner, state, type);

        SetNextSpell(rng);
    }

    private void AddBaseSpells(Player owner, ICombatState state)
    {
        CardModel[] original =
        [
            ModelDb.Card<BurningStudy>(), ModelDb.Card<Cryostasis>(),
            ModelDb.Card<Darkleech>(), ModelDb.Card<Thunderbolt>()
        ];
        var modified = AwakenedHook.ModifyBaseSpells(state, owner, original);
        foreach (var card in modified)
            CreateAndAddSpell(owner, state, card);
    }

    private void CreateAndAddSpell(Player owner, ICombatState state, CardModel canonical)
    {
        var spell = state.CreateCard(canonical, owner);
        if (AwakenedModel.IsAwakened(owner) && spell.IsUpgradable)
        {
            spell.UpgradeInternal();
            spell.FinalizeUpgradeInternal();
        }

        AddInternal(spell);
    }
}