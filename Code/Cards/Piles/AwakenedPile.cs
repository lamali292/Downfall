using BaseLib.Abstracts;
using BaseLib.Patches.Content;
using Downfall.Code.Cards.Awakened.Token;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Random;

namespace Downfall.Code.Cards.Piles;

public class AwakenedPile : CustomPile
{
    [CustomEnum] public static PileType Spellbook;
    
    public AwakenedPile() : base(Spellbook)
    {
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
    
    private CardModel? _nextSpell;

    public CardModel? NextSpell => _nextSpell;
    public void SetNextSpell(Rng rng)
    {
        var available = Cards.Where(c => c != _nextSpell).ToList();
        _nextSpell = available.Count > 0
            ? rng.NextItem(available)
            : (Cards.Count > 0 ? Cards[0] : null);
    }
    
    public bool UpgradeOnAdd { get; set; }
    
    public void Refresh(Player owner, CombatState state, Rng rng)
    {
        foreach (var card in Cards.ToList())
            card.RemoveFromState();

        AddBaseSpells(owner, state);
        SetNextSpell(rng);
    }

    private void AddBaseSpells(Player owner, CombatState state)
    {
        var baseSpells = new CardModel[]
        {
            state.CreateCard<BurningStudy>(owner),
            state.CreateCard<Cryostasis>(owner),
            state.CreateCard<Darkleech>(owner),
            state.CreateCard<Thunderbolt>(owner),
        };

        foreach (var spell in baseSpells)
        {
            if (UpgradeOnAdd && spell.IsUpgradable)
            {
                spell.UpgradeInternal();
                spell.FinalizeUpgradeInternal();
            }
            AddInternal(spell);
        }
    }
}