using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Champ.ChampCode.Enchantments;

public class Sturdy : DownfallEnchantmentModel
{
    public override bool CanEnchant(CardModel card)
    {
        return base.CanEnchant(card) && card.GainsBlock;
    }

    public override decimal EnchantBlockMultiplicative(decimal originalBlock, ValueProp props)
    {
        return !props.IsPoweredAttack() ? 1M : 2M;
    }
}