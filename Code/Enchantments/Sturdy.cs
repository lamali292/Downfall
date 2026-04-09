using Downfall.Code.Abstract;
using Downfall.Code.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.Code.Enchantments;

public class Sturdy : DownfallEnchantmentModel
{
    
    public override bool CanEnchant(CardModel card) => base.CanEnchant(card) && card.GainsBlock;

    public override decimal EnchantBlockMultiplicative(decimal originalBlock, ValueProp props)
    {
        return !props.IsPoweredAttack() ? 1M : 2M;
    }
}