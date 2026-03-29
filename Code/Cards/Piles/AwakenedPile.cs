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

    public CardModel? NextSpell { get; private set; }

    public bool UpgradeOnAdd { get; set; }

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

    public void  Refresh(Player owner)
    {
        var state = owner.Creature.CombatState;
        if (state == null) return;
        var rng = state.RunState.Rng.CombatCardSelection;
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
            state.CreateCard<Thunderbolt>(owner)
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