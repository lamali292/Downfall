using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace Champ.ChampCode.Enchantments;

public class Crowned : DownfallEnchantmentModel<Core.Champ>
{
    public override bool CanEnchant(CardModel card)
    {
        return base.CanEnchant(card) && !card.EnergyCost.CostsX && !card.HasStarCostX;
    }
    
    protected override void OnEnchant()
    {
        Card.EnergyCost.SetCustomBaseCost(0);
        if (Card.BaseStarCost > 0) Card.BaseStarCost = 0;
    }
}
