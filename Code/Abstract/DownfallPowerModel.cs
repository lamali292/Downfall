using BaseLib.Extensions;
using Downfall.Code.Character;
using Downfall.Code.Extensions;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Downfall.Code.Abstract;

public abstract class DownfallPowerModel(
    PowerType powerType = PowerType.Buff,
    PowerStackType powerStackType = PowerStackType.Counter) : ConstructedPowerModel(powerType, powerStackType)
{
    protected string IconName => Id.Entry
        .RemovePrefix()
        .ToLowerInvariant();

    public override string CustomPackedIconPath => $"{IconName}.png".DownfallPowerImagePath();
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

public abstract class AutomatonPowerModel(
    PowerType powerType = PowerType.Buff,
    PowerStackType powerStackType = PowerStackType.Counter) : DownfallPowerModel<Automaton>(powerType, powerStackType);

public abstract class AwakenedPowerModel(
    PowerType powerType = PowerType.Buff,
    PowerStackType powerStackType = PowerStackType.Counter) : DownfallPowerModel<Awakened>(powerType, powerStackType);

public abstract class ChampPowerModel(
    PowerType powerType = PowerType.Buff,
    PowerStackType powerStackType = PowerStackType.Counter) : DownfallPowerModel<Champ>(powerType, powerStackType);

public abstract class CollectorPowerModel(
    PowerType powerType = PowerType.Buff,
    PowerStackType powerStackType = PowerStackType.Counter) : DownfallPowerModel<Collector>(powerType, powerStackType);

public abstract class GremlinsPowerModel(
    PowerType powerType = PowerType.Buff,
    PowerStackType powerStackType = PowerStackType.Counter) : DownfallPowerModel<Gremlins>(powerType, powerStackType);

public abstract class GuardianPowerModel(
    PowerType powerType = PowerType.Buff,
    PowerStackType powerStackType = PowerStackType.Counter) : DownfallPowerModel<Guardian>(powerType, powerStackType);

public abstract class HexaghostPowerModel(
    PowerType powerType = PowerType.Buff,
    PowerStackType powerStackType = PowerStackType.Counter) : DownfallPowerModel<Hexaghost>(powerType, powerStackType);

public abstract class SlimeBossPowerModel(
    PowerType powerType = PowerType.Buff,
    PowerStackType powerStackType = PowerStackType.Counter) : DownfallPowerModel<SlimeBoss>(powerType, powerStackType);

public abstract class SneckoPowerModel(
    PowerType powerType = PowerType.Buff,
    PowerStackType powerStackType = PowerStackType.Counter) : DownfallPowerModel<Snecko>(powerType, powerStackType);