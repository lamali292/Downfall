using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace Champ.ChampCode.Enchantments;

public class Crowned : DownfallEnchantmentModel<Core.Champ>
{
    public override bool CanEnchant(CardModel card)
    {
        return base.CanEnchant(card) && !card.EnergyCost.CostsX;
        ;
    }

    protected override void OnEnchant()
    {
        Card.EnergyCost.UpgradeBy(-Card.EnergyCost.GetWithModifiers(CostModifiers.None));
        Card.EnergyCost.FinalizeUpgrade();
    }
}