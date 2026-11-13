using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.Models;

namespace Champ.ChampCode.Enchantments;

public class Sturdy : DownfallEnchantmentModel<Core.Champ>
{
    public override bool CanEnchant(CardModel card)
    {
        return base.CanEnchant(card) && card.GainsBlock;
    }

    public override decimal EnchantBlockMultiplicative(decimal originalBlock)
    {
        return 2;
    }
}