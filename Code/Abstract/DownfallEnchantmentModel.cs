using BaseLib.Abstracts;
using BaseLib.Extensions;
using Downfall.Code.Extensions;

namespace Downfall.Code.Abstract;

public class DownfallEnchantmentModel : CustomEnchantmentModel
{
    protected override string CustomIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".EnchantmentPath();
}