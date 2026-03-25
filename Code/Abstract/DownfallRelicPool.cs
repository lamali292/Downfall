using BaseLib.Abstracts;
using Downfall.Code.Character;
using Godot;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Abstract;

public abstract class DownfallRelicPool<T> : CustomRelicPoolModel
    where T : DownfallCharacterModel
{
    private static T Character => ModelDb.Character<T>();

    public override string EnergyColorName => Character.CharId!;
    public override Color LabOutlineColor => Character.LabOutlineColor;
}

public class AutomatonRelicPool : DownfallRelicPool<Automaton>;

public class AwakenedRelicPool : DownfallRelicPool<Awakened>;

public class ChampRelicPool : DownfallRelicPool<Champ>;

public class CollectorRelicPool : DownfallRelicPool<Collector>;

public class GremlinsRelicPool : DownfallRelicPool<Gremlins>;

public class GuardianRelicPool : DownfallRelicPool<Guardian>;

public class HexaghostRelicPool : DownfallRelicPool<Hexaghost>;

public class SlimeBossRelicPool : DownfallRelicPool<SlimeBoss>;

public class SneckoRelicPool : DownfallRelicPool<Snecko>;