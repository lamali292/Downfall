using BaseLib.Abstracts;
using Downfall.Code.Character;
using Godot;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Abstract;

public abstract class DownfallCardPool<T> : CustomCardPoolModel
    where T : DownfallCharacterModel<T>
{
    private static T Character => ModelDb.Character<T>();
    public override string Title => Character.CharId!;

    public override string? BigEnergyIconPath =>
        $"res://Downfall/character/energy_counters/icon/{Title.ToSnakeCase()}_energy_icon.png";

    public override string? TextEnergyIconPath =>
        $"res://Downfall/character/energy_counters/text/text_{Title.ToSnakeCase()}_energy_icon.png";

    public override float H => Character.CardHsv.H;
    public override float S => Character.CardHsv.S;
    public override float V => Character.CardHsv.V;

    public override Color DeckEntryCardColor => Character.DeckEntryCardColor;
    public override bool IsColorless => false;
}

public class AutomatonCardPool : DownfallCardPool<Automaton>;

public class AwakenedCardPool : DownfallCardPool<Awakened>;

public class ChampCardPool : DownfallCardPool<Champ>;

public class CollectorCardPool : DownfallCardPool<Collector>;

public class GremlinsCardPool : DownfallCardPool<Gremlins>;

public class GuardianCardPool : DownfallCardPool<Guardian>;

public class HexaghostCardPool : DownfallCardPool<Hexaghost>;

public class SlimeBossCardPool : DownfallCardPool<SlimeBoss>;

public class SneckoCardPool : DownfallCardPool<Snecko>;