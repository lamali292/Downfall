using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace Downfall.DownfallCode.Abstract;

public abstract class DownfallRelicModel<T>(RelicRarity rarity) : ConstructedRelicModel(rarity)
    where T : DownfallCharacterModel
{
    private string IconName => Id.Entry
        .RemovePrefix()
        .ToLowerInvariant();

    public override string PackedIconPath => $"{IconName}.tres".TresRelicImagePath<T>();
    protected override string PackedIconOutlinePath => $"{IconName}_outline.tres".TresRelicImagePath<T>();
    protected override string BigIconPath => $"{IconName}.png".BigRelicImagePath<T>();
}