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
        Register("RUINA2-ALRIUNE_ELITE", "RUINA2-FAINT_AROMA", "RUINA2-DA_CAPO", "RUINA2-FRAGMENTS_FROM_SOMEWHERE", "RUINA2-PLEASURE", "RUINA2-OUR_GALAXY");
        Register("RUINA2-HELPERS_ELITE", "RUINA2-GRINDER", "RUINA2-MAGIC_BULLET", "RUINA2-REGRET", "RUINA2-HARMONY", "RUINA2-SOLEMN_LAMENT");
        Register("RUINA2-LAETITIA_ELITE", "RUINA2-LAETITIA", "RUINA2-BLACK_SWAN", "RUINA2-RED_EYES", "RUINA2-SANGUINE_DESIRE", "RUINA2-TODAYS_EXPRESSION");
        Register("RUINA2-FAIRY_BOSS", "RUINA2-WINGBEAT", "RUINA2-FOURTH_MATCH_FLAME", "RUINA2-GREEN_STEM", "RUINA2-THE_FORGOTTEN", "RUINA2-HORNET");
        Register("RUINA2-NOTHING_DER_BOSS", "RUINA2-GRINDER", "RUINA2-MAGIC_BULLET", "RUINA2-REGRET", "RUINA2-HARMONY", "RUINA2-SOLEMN_LAMENT");
        Register("RUINA2-BLACK_SWAN_BOSS", "RUINA2-LAETITIA", "RUINA2-BLACK_SWAN", "RUINA2-RED_EYES", "RUINA2-SANGUINE_DESIRE", "RUINA2-TODAYS_EXPRESSION");
        Register("RUINA2-ORCHESTRA_BOSS", "RUINA2-FAINT_AROMA", "RUINA2-DA_CAPO", "RUINA2-FRAGMENTS_FROM_SOMEWHERE", "RUINA2-PLEASURE", "RUINA2-OUR_GALAXY");
        Register("RUINA2-MOUNTAIN_ELITE", "RUINA2-SMILE", "RUINA2-CRIMSON_SCAR", "RUINA2-MIMICRY", "RUINA2-THIRST", "RUINA2-COBALT_SCAR");
        Register("RUINA2-WRATH_ELITE", "RUINA2-BLIND_RAGE", "RUINA2-LOVE_AND_HATE", "RUINA2-GOLD_RUSH", "RUINA2-NIHIL", "RUINA2-SWORD_SHARPENED");
        Register("RUINA2-ROAD_HOME_ELITE", "RUINA2-HOMING_INSTINCT", "RUINA2-HARVEST", "RUINA2-FALSE_THRONE", "RUINA2-LUMBER", "RUINA2-FADED_MEMORIES");
        Register("RUINA2-RED_WOLF_BOSS", "RUINA2-SMILE", "RUINA2-CRIMSON_SCAR", "RUINA2-MIMICRY", "RUINA2-THIRST", "RUINA2-COBALT_SCAR");
        Register("RUINA2-JESTER_BOSS", "RUINA2-BLIND_RAGE", "RUINA2-LOVE_AND_HATE", "RUINA2-GOLD_RUSH", "RUINA2-NIHIL", "RUINA2-SWORD_SHARPENED");
        Register("RUINA2-OZ_BOSS", "RUINA2-HOMING_INSTINCT", "RUINA2-HARVEST", "RUINA2-FALSE_THRONE", "RUINA2-LUMBER", "RUINA2-FADED_MEMORIES");
        Register("RUINA2-BIG_BIRD_ELITE", "RUINA2-LAMP", "RUINA2-TWILIGHT", "RUINA2-APOCALYPSE", "RUINA2-JUSTITIA", "RUINA2-BEAK");
        Register("RUINA2-BLUE_STAR_ELITE", "RUINA2-SOUND_OF_A_STAR", "RUINA2-PENITENCE", "RUINA2-DEAD_SILENCE", "RUINA2-HEAVEN", "RUINA2-PARADISE_LOST");
        Register("RUINA2-SNOW_QUEEN_ELITE", "RUINA2-FROST_SPLINTER", "RUINA2-REMORSE", "RUINA2-WRIST_CUTTER", "RUINA2-ASPIRATION", "RUINA2-MARIONETTE");
        Register("RUINA2-TWILIGHT_BOSS", "RUINA2-LAMP", "RUINA2-TWILIGHT", "RUINA2-APOCALYPSE", "RUINA2-JUSTITIA", "RUINA2-BEAK");
        Register("RUINA2-WHITE_NIGHT_BOSS", "RUINA2-SOUND_OF_A_STAR", "RUINA2-PENITENCE", "RUINA2-DEAD_SILENCE", "RUINA2-HEAVEN", "RUINA2-PARADISE_LOST");
        Register("RUINA2-SILENT_GIRL_BOSS", "RUINA2-FROST_SPLINTER", "RUINA2-REMORSE", "RUINA2-WRIST_CUTTER", "RUINA2-ASPIRATION", "RUINA2-MARIONETTE");
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
