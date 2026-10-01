using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Extensions;

/// <summary>
/// The single place that reads a card's cost from the game. Callers ask domain questions here and never
/// pick a game accessor (resolved / amount-to-spend / with-modifiers / canonical) or spell the X checks
/// themselves, so a game-API change only needs editing in this file. The decisions themselves live in
/// <see cref="CardCostRules"/>.
/// </summary>
public static class CardCostExtensions
{
    extension(CardModel card)
    {
        /// <summary>Card costs X energy: it spends all energy (and whatever else the owner converts) when played.</summary>
        public bool IsXEnergy => card.EnergyCost.CostsX;

        /// <summary>Not X-energy and not X-star. See <see cref="CardCostRules.HasNumericCost"/>.</summary>
        public bool HasNumericCost => !card.EnergyCost.CostsX && !card.HasStarCostX;

        /// <summary>The energy the player pays right now (X cards: all current energy, never negative).</summary>
        public int EffectiveCost => CardCostRules.EffectiveCost(card.EnergyCost.CostsX,
            card.EnergyCost.CostsX ? card.EnergyCost.GetAmountToSpend() : 0,
            card.EnergyCost.GetWithModifiers(CostModifiers.All));

        /// <summary>
        /// The current cost with every modifier applied. Unlike <see cref="EffectiveCost"/> this keeps the
        /// negative cost of unplayable cards instead of clamping to zero.
        /// </summary>
        public int ModifiedCost => card.EnergyCost.GetWithModifiers(CostModifiers.All);

        /// <summary>The cost printed on the card, ignoring every modifier. X cards report 0.</summary>
        public int PrintedCost => card.EnergyCost.Canonical;

        /// <summary>A numeric cost that is currently zero.</summary>
        public bool IsFreeNow => card is { HasNumericCost: true, ModifiedCost: 0 };
    }
}
