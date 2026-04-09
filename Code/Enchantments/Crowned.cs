using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Enchantments;

public class Crowned : DownfallEnchantmentModel
{
    protected override void OnEnchant()
    {
        Card.EnergyCost.UpgradeBy(-Card.EnergyCost.GetWithModifiers(CostModifiers.None));
    }

}