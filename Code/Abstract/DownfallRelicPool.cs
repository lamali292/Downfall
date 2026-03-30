using BaseLib.Abstracts;
using Downfall.Code.Character;
using Godot;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Abstract;

public abstract class DownfallRelicPool<T> : CustomRelicPoolModel
    where T : DownfallCharacterModel
{
    private static T Character => ModelDb.Character<T>();

    //public override string EnergyColorName => Character.CharId!;
    private static string Title => Character.CharId!;
    public override Color LabOutlineColor => Character.LabOutlineColor;

    public override string? BigEnergyIconPath =>
        $"res://Downfall/character/energy_counters/icon/{Title.ToSnakeCase()}_energy_icon.png";

    public override string? TextEnergyIconPath =>
        $"res://Downfall/character/energy_counters/text/text_{Title.ToSnakeCase()}_energy_icon.png";
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