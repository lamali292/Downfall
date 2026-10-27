namespace Collector.CollectorCode.Rewards;

/// <summary>
/// Maps encounters from other mods (by <see cref="MegaCrit.Sts2.Core.Models.ModelId.Entry"/>) to the Collector
/// cards that can be offered as their Collectible reward. Other mods may call <see cref="Register"/> to add
/// their own encounters; Collector itself ships the entries below.
/// </summary>
public static class ModdedEncounterCollectibles
{
    private static readonly Dictionary<string, string[]> Cards = new();

    static ModdedEncounterCollectibles()
    {
        Register("RUINA2-FAIRY_BOSS","RUINA2-WINGBEAT");
        Register("RUINA2-NOTHING_DER_BOSS","RUINA2-MAGIC_BULLET");
        Register("RUINA2-BLACK_SWAN_BOSS","RUINA2-BLACK_SWAN");
        Register("RUINA2-ORCHESTRA_BOSS","RUINA2-DA_CAPO");
        Register("RUINA2-RED_WOLF_BOSS","RUINA2-CRIMSON_SCAR");
        Register("RUINA2-JESTER_BOSS", "RUINA2-NIHIL");
        Register("RUINA2-OZ_BOSS", "RUINA2-FALSE_THRONE");
        Register("RUINA2-TWILIGHT_BOSS", "RUINA2-TWILIGHT", "RUINA2-APOCALYPSE");
        Register("RUINA2-WHITE_NIGHT_BOSS","RUINA2-PARADISE_LOST");
        Register("RUINA2-SILENT_GIRL_BOSS", "RUINA2-REMORSE");
        Register("ACTSFROMTHEPAST-SLIME_BOSS_BOSS", "SLIMEBOSS-PREPARE_CRUSH");
        Register("ACTSFROMTHEPAST-GUARDIAN_BOSS", "GUARDIAN-BODY_CRASH");
        Register("ACTSFROMTHEPAST-HEXAGHOST_BOSS", "HEXAGHOST-ETHER_STEP");
        Register("ACTSFROMTHEPAST-BRONZE_AUTOMATON_BOSS", "AUTOMATON-HYPER_BEAM_AUTOMATON");
        Register("ACTSFROMTHEPAST-CHAMP_BOSS", "CHAMP-MURDER_STRIKE");
        Register("ACTSFROMTHEPAST-AWAKENED_ONE_BOSS", "AWAKENED-MURDER");
    }

    /// <summary>Registers (or replaces) the card entries offered for an encounter entry.</summary>
    public static void Register(string encounterEntry, params string[] cardEntries)
    {
        Cards[encounterEntry] = cardEntries;
    }

    /// <summary>Returns a fresh copy of the card entries for the encounter, or null when none are registered.</summary>
    public static List<string>? TryGetCardEntries(string encounterEntry)
    {
        return Cards.TryGetValue(encounterEntry, out var entries) ? entries.ToList() : null;
    }
}
