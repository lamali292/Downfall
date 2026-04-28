using BaseLib.Abstracts;
using BaseLib.Extensions;
using Downfall.DownfallCode.Extensions;

namespace Downfall.DownfallCode.Abstract;

public abstract class DownfallEnchantmentModel<T> : CustomEnchantmentModel where T : DownfallCharacterModel
{
    protected override string CustomIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".EnchantmentPath<T>();
}