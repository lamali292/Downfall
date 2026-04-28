using BaseLib.Abstracts;
using BaseLib.Extensions;
using Downfall.DownfallCode.Extensions;

namespace Downfall.DownfallCode.Abstract;

public abstract class DownfallRelicModel<T> : ConstructedRelicModel
    where T : DownfallCharacterModel
{
    private string IconName => Id.Entry
        .RemovePrefix()
        .ToLowerInvariant();

    public override string PackedIconPath => $"{IconName}.tres".TresRelicImagePath<T>();
    protected override string PackedIconOutlinePath => $"{IconName}_outline.tres".TresRelicImagePath<T>();
    protected override string BigIconPath => $"{IconName}.png".BigRelicImagePath<T>();
}