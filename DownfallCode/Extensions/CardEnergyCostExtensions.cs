using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.DownfallCode.Extensions;

public static class CardEnergyCostExtensions
{
    extension(CardEnergyCost energyCost)
    {
        /// <summary>A numeric cost that is currently zero.</summary>
        public bool Is0Cost => !energyCost.CostsX && energyCost.GetAmountToSpend() == 0;
        public int Modified => energyCost.GetWithModifiers(CostModifiers.All);
    }
}