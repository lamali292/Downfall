using BaseLib.Abstracts;
using BaseLib.Extensions;
using Downfall.Code.Character;
using Downfall.Code.Extensions;

namespace Downfall.Code.Abstract;

public abstract class DownfallPowerModel<T> : CustomPowerModel
    where T : DownfallCharacterModel<T>
{
    private string IconName => Id.Entry
        .RemovePrefix()
        .RemoveSuffix("Power")
        .ToLowerInvariant();

    public override string CustomPackedIconPath => $"{IconName}.png".PowerImagePath<T>();
    public override string CustomBigIconPath => $"{IconName}.png".BigPowerImagePath<T>();
}

public abstract class AutomatonPowerModel : DownfallPowerModel<Automaton>;

public abstract class AwakenedPowerModel : DownfallPowerModel<Awakened>;

public abstract class ChampPowerModel : DownfallPowerModel<Champ>;

public abstract class CollectorPowerModel : DownfallPowerModel<Collector>;

public abstract class GremlinsPowerModel : DownfallPowerModel<Gremlins>;

public abstract class GuardianPowerModel : DownfallPowerModel<Guardian>;

public abstract class HexaghostPowerModel : DownfallPowerModel<Hexaghost>;

public abstract class SlimeBossPowerModel : DownfallPowerModel<SlimeBoss>;

public abstract class SneckoPowerModel : DownfallPowerModel<Snecko>;