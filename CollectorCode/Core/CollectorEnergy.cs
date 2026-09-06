using BaseLib.Utils;
using Collector.CollectorCode.Interfaces;
using Collector.CollectorCode.Vfx;
using Downfall.DownfallCode.Abstract;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Core;

public class CollectorEnergy : CardResource
{
    public override string ResourceName => "Collector Energy";
    public override Vector2 UiPosition => new(80f, 80f);
    public override Vector2 UiScale => new(0.6f, 0.6f);

    protected override bool InteractsWithEnergy => true;
    
    private readonly SpireField<CardModel, int> _lastSpent = new(() => 0);

    public override Control CreateCounter(Player player)
    {
        return NCollectorEnergyCounter.Create(player);
    }

    public override (int energySpent, int starsSpent) HandleSpending(CardModel card)
    {
        var player = card.Owner;
        var cost = card.EnergyCost.GetAmountToSpend();

        if (UsesResourceExclusively(card))
        {
            if (!CanAfford(player, cost))
            {
                _lastSpent[card] = 0;
                return (0, 0);
            }

            Spend(player, cost);
            _lastSpent[card] = cost;
            return (0, 0);
        }

        var energy = player.PlayerCombatState?.Energy ?? 0;
        if (energy >= cost)
        {
            _lastSpent[card] = 0;
            return (cost, 0);
        }

        var deficit = cost - energy;
        var available = Get(player);
        var cover = Math.Min(deficit, available);

        if (cover > 0) Spend(player, cover);
        _lastSpent[card] = cover;
        return (energy, 0);
    }

    public override (bool hasResources, UnplayableReason reason) CheckResources(CardModel card)
    {
        var player = card.Owner;
        var cost = card.EnergyCost.GetWithModifiers(CostModifiers.All);

        if (UsesResourceExclusively(card))
            return Get(player) >= cost ? (true, UnplayableReason.None) : (false, UnplayableReason.EnergyCostTooHigh);

        var energy = player.PlayerCombatState?.Energy ?? 0;
        var totalAvailable = energy + Get(player);

        return totalAvailable >= cost ? (true, UnplayableReason.None) : (false, UnplayableReason.EnergyCostTooHigh);
    }

    public override bool ShouldHandleSpending(CardModel card)
    {
        return true;
    }

    public override bool ShouldHandleResourceCheck(CardModel card)
    {
        return true;
    }

    public override bool UsesResourceExclusively(CardModel card)
    {
        return card is IUsesCollectorEnergyOnly;
    }

    public override bool ShouldPlay(CardModel card, AutoPlayType autoPlayType)
    {
        if (card.Owner.PlayerCombatState == null) return true;
        var reserve = Get(card.Owner);
        if (reserve <= 0) return true;
        var cost = card.EnergyCost.GetWithModifiers(CostModifiers.All);
        return card.Owner.PlayerCombatState.Energy + reserve >= cost;
    }
    
    public bool WasSpentOn(CardModel card) => _lastSpent[card] > 0;
    public int AmountSpentOn(CardModel card) => _lastSpent[card];
}