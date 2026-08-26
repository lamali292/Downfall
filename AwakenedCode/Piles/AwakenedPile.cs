using Awakened.AwakenedCode.Cards.Token;
using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Events;
using Awakened.AwakenedCode.Vfx;
using BaseLib.Abstracts;
using BaseLib.Patches.Content;
using Downfall.DownfallCode.Utils.UI;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Awakened.AwakenedCode.Piles;

public class AwakenedPile() : CustomPile(Spellbook)
{
    [CustomEnum] public static PileType Spellbook;

    private readonly List<CardModel> _dynamicTypes = [];
    

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
        return NCustomCombatCardPile.GetPositionFor<NSpellbookButton>();
    }
    
    private Type? _nextSpellType;

    public CardModel? NextSpell { get; private set; }

    public void SetNextSpell(Player player)
    {
        var available = Cards
            .Where(c => c.GetType() != _nextSpellType)
            .ToList();

        NextSpell = available.Count > 0
            ? player.RunState.Rng.CombatCardSelection.NextItem(available)
            : Cards.Count > 0
                ? Cards[0]
                : null;

        _nextSpellType = NextSpell?.GetType();
    }

    public void Refresh(Player owner)
    {
        var state = owner.Creature.CombatState;
        if (state == null) return;
        var previousType = _nextSpellType ?? NextSpell?.GetType();

        foreach (var card in Cards.ToList())
            card.RemoveFromState();

        AddBaseSpells(owner, state);

        foreach (var type in _dynamicTypes)
            CreateAndAddSpell(owner, state, type);
        
        if (previousType == null) return;
        NextSpell = Cards.FirstOrDefault(c => c.GetType() == previousType);
        _nextSpellType = NextSpell?.GetType();
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