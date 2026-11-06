using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Interfaces;

public interface IEnchantRestrictedCard
{
    bool CanBeEnchantedWith(EnchantmentModel enchantment);
}