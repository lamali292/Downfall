using BaseLib.Abstracts;
using Downfall.Code.Character;
using Godot;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Abstract;

public abstract class DownfallPotionPool<T> : CustomPotionPoolModel
    where T : DownfallCharacterModel
{

    private static T Character => ModelDb.Character<T>();

    public override string EnergyColorName => Character.CharId!;
    public override Color LabOutlineColor => Character.LabOutlineColor;
}

public class AutomatonPotionPool : DownfallPotionPool<Automaton>;

public class AwakenedPotionPool : DownfallPotionPool<Awakened>;

public class ChampPotionPool : DownfallPotionPool<Champ>;

public class CollectorPotionPool : DownfallPotionPool<Collector>;

public class GremlinsPotionPool : DownfallPotionPool<Gremlins>;

public class GuardianPotionPool : DownfallPotionPool<Guardian>;

public class HexaghostPotionPool : DownfallPotionPool<Hexaghost>;

public class SlimeBossPotionPool : DownfallPotionPool<SlimeBoss>;

public class SneckoPotionPool : DownfallPotionPool<Snecko>;