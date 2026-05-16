using BaseLib.Extensions;
using Downfall.DownfallCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Downfall.DownfallCode.Abstract;

public abstract class DownfallPowerModel(
    PowerType powerType = PowerType.Buff,
    PowerStackType powerStackType = PowerStackType.Counter) : ConstructedPowerModel(powerType, powerStackType)
{
    protected string IconName => Id.Entry
        .RemovePrefix()
        .ToLowerInvariant();

    public override string CustomPackedIconPath => $"{IconName}.tres".DownfallPowerImagePath();
    public override string CustomBigIconPath => $"{IconName}.png".DownfallBigPowerImagePath();
}

public abstract class DownfallPowerModel<T>(
    PowerType powerType = PowerType.Buff,
    PowerStackType powerStackType = PowerStackType.Counter) : DownfallPowerModel(powerType, powerStackType)
    where T : DownfallCharacterModel
{
    public override string CustomPackedIconPath => $"{IconName}.tres".PowerImagePath<T>();
    public override string CustomBigIconPath => $"{IconName}.png".BigPowerImagePath<T>();
}