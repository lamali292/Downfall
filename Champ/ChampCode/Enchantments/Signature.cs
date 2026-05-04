using Champ.ChampCode.CustomEnums;
using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.Models;

namespace Champ.ChampCode.Enchantments;

public class Signature : DownfallEnchantmentModel<Core.Champ>
{
    public override bool CanEnchant(CardModel card)
    {
        return card.Tags.Contains(ChampTag.Finisher);
    }
}