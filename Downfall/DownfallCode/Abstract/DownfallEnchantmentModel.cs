using BaseLib.Abstracts;
using BaseLib.Extensions;
using Downfall.DownfallCode.Extensions;

namespace Downfall.DownfallCode.Abstract;

public class DownfallEnchantmentModel : CustomEnchantmentModel
{
    protected override string CustomIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".EnchantmentPath();
}