using BaseLib.Utils;
using Downfall.Code.Enchantments;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Keywords;

public class DownfallTip
{
    public static TooltipSource Enchantment<T>() where T : EnchantmentModel
    {
        return new TooltipSource(_ => ModelDb.Enchantment<Crowned>().HoverTip);
    }
}