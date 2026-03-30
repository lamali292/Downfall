using BaseLib.Abstracts;
using BaseLib.Extensions;
using Downfall.Code.Character;
using Downfall.Code.Extensions;

namespace Downfall.Code.Abstract;

public abstract class DownfallRelicModel<T> : CustomRelicModel
where T : DownfallCharacterModel
{
    private string IconName => Id.Entry
        .RemovePrefix()
        .ToLowerInvariant();
    public override string PackedIconPath => $"{IconName}.tres".TresRelicImagePath();
    protected override string PackedIconOutlinePath => $"{IconName}_outline.tres".TresRelicImagePath();
    protected override string BigIconPath => $"{IconName}.png".BigRelicImagePath();
}

public abstract class AutomatonRelicModel : DownfallRelicModel<Automaton>;

public abstract class AwakenedRelicModel : DownfallRelicModel<Awakened>;

public abstract class ChampRelicModel : DownfallRelicModel<Champ>;

public abstract class CollectorRelicModel : DownfallRelicModel<Collector>;

public abstract class GremlinsRelicModel : DownfallRelicModel<Gremlins>;

public abstract class GuardianRelicModel : DownfallRelicModel<Guardian>;

public abstract class HexaghostRelicModel : DownfallRelicModel<Hexaghost>;

public abstract class SlimeBossRelicModel : DownfallRelicModel<SlimeBoss>;

public abstract class SneckoRelicModel : DownfallRelicModel<Snecko>;