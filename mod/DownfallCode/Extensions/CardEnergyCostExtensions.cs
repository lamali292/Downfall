using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.DownfallCode.Extensions;

public static class CardEnergyCostExtensions
{
    extension(CardEnergyCost energyCost)
    {
        /// <summary>
        /// A numeric cost that is currently zero. Reads the unclamped modified cost: GetAmountToSpend clamps
        /// negative costs to 0, which would make unplayable cards like Ascender's Bane look free.
        /// </summary>
        public bool Is0Cost => energyCost is { CostsX: false, Modified: 0 };
        public int Modified => energyCost.GetWithModifiers(CostModifiers.All);
    }

}