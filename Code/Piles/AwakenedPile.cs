using BaseLib.Abstracts;
using BaseLib.Patches.Content;
using Downfall.Code.Cards.Awakened.Token;
using Downfall.Code.Core;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Random;

namespace Downfall.Code.Piles;

public class AwakenedPile() : CustomPile(Spellbook)
{
    [CustomEnum] public static PileType Spellbook;
    
    private readonly List<Type> _dynamicTypes = [];

    public CardModel? NextSpell { get; private set; }
    
    
    public void AddPersistentType(Type type)
    {
        _dynamicTypes.Add(type);
    }

    public override bool CardShouldBeVisible(CardModel card)
    {
        return false;
    }


    public override Vector2 GetTargetPosition(CardModel model, Vector2 size)
    {
        var creatureNode = NCombatRoom.Instance?.GetCreatureNode(model.Owner.Creature);
        if (creatureNode == null) return Vector2.Zero;

        var index = Cards.IndexOf(model);
        var totalWidth = Cards.Count * (size.X + 8f);
        var startX = creatureNode.GlobalPosition.X - totalWidth / 2f + index * (size.X + 8f);
        var y = creatureNode.GlobalPosition.Y - creatureNode.Size.Y / 2f - size.Y - 16f;

        return new Vector2(startX, y);
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

    private void AddBaseSpells(Player owner, CombatState state)
    {
        Type[] baseTypes =
        [
            typeof(BurningStudy), typeof(Cryostasis),
            typeof(Darkleech), typeof(Thunderbolt)
        ];

        foreach (var type in baseTypes)
            CreateAndAddSpell(owner, state, type);
    }
    
    private void CreateAndAddSpell(Player owner, CombatState state, Type type)
    {
        var id = ModelDb.GetId(type);
        var model = ModelDb.GetById<CardModel>(id);
        var spell = state.CreateCard(model, owner);
        if ( AwakenedModel.IsAwakened(owner) && spell.IsUpgradable)
        {
            spell.UpgradeInternal();
            spell.FinalizeUpgradeInternal();
        }

        AddInternal(spell);
    }
}