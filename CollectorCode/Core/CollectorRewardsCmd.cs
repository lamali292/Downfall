using BaseLib.Extensions;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Events;
using Collector.CollectorCode.Extensions;
using Collector.CollectorCode.Powers;
using Downfall.DownfallCode.Commands;
using Downfall.DownfallCode.Compatibility;
using Godot;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace Collector.CollectorCode.Core;

public static class CollectorRewardsCmd
{
    
    
    

    
    public static bool TryAddCollectiblesReward(RelicModel relic, Player player, List<CardCreationResult> cardRewardOptions, CardCreationOptions creationOptions, Action<CardModel>? action = null)
    {
        if (creationOptions.Source != CardCreationSource.Encounter
            || !creationOptions.Flags.HasFlag(CardCreationFlags.IsCardReward)
            )
            return false;
        // maybe add || !creationOptions.Flags.HasFlag(CardCreationFlags.IsFromCombat) back
        if (player.RunState.CurrentRoom is not CombatRoom { RoomType: RoomType.Elite or RoomType.Boss } room)
            return false;

        var encounterId = room.Encounter.Id;
        var pool = ModelDb.CardPool<CollectibleCardPool>().AllCards.ToList();
        // get our collectibles
        var model = pool.FirstOrDefault(c => c is ICollectible g && g.GetEncounterModel()?.Id == encounterId);
        // fallback to other mods
        model ??= GetCardForModdedEnemy(player, encounterId);
        // final fallback. pick random elite or boss with the same act number.
        model ??= player.RunState.Rng.Niche.NextItem(pool
                .Where(c => c is ICollectible g && 
                            (g.Act()?.ActNumber() ?? -1) == room.Act.ActNumber() &&
                            g.RoomType() == room.RoomType)
        );
        if (model is null) return false;
        var card = player.RunState.CreateCard(model, player);
        action?.Invoke(card);
        var result = new CardCreationResult(card);
        result.ModifyCard(card, relic);
        cardRewardOptions.Add(result);
        return true;
    }

    private static CardModel? GetCardForModdedEnemy(Player player, ModelId encounterId)
    {
        var moddedEnemyMap = new Dictionary<string, List<string>>
        {
            { "RUINA2-ALRIUNE_ELITE", ["RUINA2-FAINT_AROMA", "RUINA2-DA_CAPO", "RUINA2-FRAGMENTS_FROM_SOMEWHERE", "RUINA2-PLEASURE", "RUINA2-OUR_GALAXY"] },
            { "RUINA2-HELPERS_ELITE", ["RUINA2-GRINDER", "RUINA2-MAGIC_BULLET", "RUINA2-REGRET", "RUINA2-HARMONY", "RUINA2-SOLEMN_LAMENT"] },
            { "RUINA2-LAETITIA_ELITE", ["RUINA2-LAETITIA", "RUINA2-BLACK_SWAN", "RUINA2-RED_EYES", "RUINA2-SANGUINE_DESIRE", "RUINA2-TODAYS_EXPRESSION"] },
            { "RUINA2-FAIRY_BOSS", ["RUINA2-WINGBEAT", "RUINA2-FOURTH_MATCH_FLAME", "RUINA2-GREEN_STEM", "RUINA2-THE_FORGOTTEN", "RUINA2-HORNET"] },
            { "RUINA2-NOTHING_DER_BOSS", ["RUINA2-GRINDER", "RUINA2-MAGIC_BULLET", "RUINA2-REGRET", "RUINA2-HARMONY", "RUINA2-SOLEMN_LAMENT"] },
            { "RUINA2-BLACK_SWAN_BOSS", ["RUINA2-LAETITIA", "RUINA2-BLACK_SWAN", "RUINA2-RED_EYES", "RUINA2-SANGUINE_DESIRE", "RUINA2-TODAYS_EXPRESSION"] },
            { "RUINA2-ORCHESTRA_BOSS", ["RUINA2-FAINT_AROMA", "RUINA2-DA_CAPO", "RUINA2-FRAGMENTS_FROM_SOMEWHERE", "RUINA2-PLEASURE", "RUINA2-OUR_GALAXY"] },
            { "RUINA2-MOUNTAIN_ELITE", ["RUINA2-SMILE", "RUINA2-CRIMSON_SCAR", "RUINA2-MIMICRY", "RUINA2-THIRST", "RUINA2-COBALT_SCAR"] },
            { "RUINA2-WRATH_ELITE", ["RUINA2-BLIND_RAGE", "RUINA2-LOVE_AND_HATE", "RUINA2-GOLD_RUSH", "RUINA2-NIHIL", "RUINA2-SWORD_SHARPENED"] },
            { "RUINA2-ROAD_HOME_ELITE", ["RUINA2-HOMING_INSTINCT", "RUINA2-HARVEST", "RUINA2-FALSE_THRONE", "RUINA2-LUMBER", "RUINA2-FADED_MEMORIES"] },
            { "RUINA2-RED_WOLF_BOSS", ["RUINA2-SMILE", "RUINA2-CRIMSON_SCAR", "RUINA2-MIMICRY", "RUINA2-THIRST", "RUINA2-COBALT_SCAR"] },
            { "RUINA2-JESTER_BOSS", ["RUINA2-BLIND_RAGE", "RUINA2-LOVE_AND_HATE", "RUINA2-GOLD_RUSH", "RUINA2-NIHIL", "RUINA2-SWORD_SHARPENED"] },
            { "RUINA2-OZ_BOSS", ["RUINA2-HOMING_INSTINCT", "RUINA2-HARVEST", "RUINA2-FALSE_THRONE", "RUINA2-LUMBER", "RUINA2-FADED_MEMORIES"] },
            { "RUINA2-BIG_BIRD_ELITE", ["RUINA2-LAMP", "RUINA2-TWILIGHT", "RUINA2-APOCALYPSE", "RUINA2-JUSTITIA", "RUINA2-BEAK"] },
            { "RUINA2-BLUE_STAR_ELITE", ["RUINA2-SOUND_OF_A_STAR", "RUINA2-PENITENCE", "RUINA2-DEAD_SILENCE", "RUINA2-HEAVEN", "RUINA2-PARADISE_LOST"] },
            { "RUINA2-SNOW_QUEEN_ELITE", ["RUINA2-FROST_SPLINTER", "RUINA2-REMORSE", "RUINA2-WRIST_CUTTER", "RUINA2-ASPIRATION", "RUINA2-MARIONETTE"] },
            { "RUINA2-TWILIGHT_BOSS", ["RUINA2-LAMP", "RUINA2-TWILIGHT", "RUINA2-APOCALYPSE", "RUINA2-JUSTITIA", "RUINA2-BEAK"] },
            { "RUINA2-WHITE_NIGHT_BOSS", ["RUINA2-SOUND_OF_A_STAR", "RUINA2-PENITENCE", "RUINA2-DEAD_SILENCE", "RUINA2-HEAVEN", "RUINA2-PARADISE_LOST"] },
            { "RUINA2-SILENT_GIRL_BOSS", ["RUINA2-FROST_SPLINTER", "RUINA2-REMORSE", "RUINA2-WRIST_CUTTER", "RUINA2-ASPIRATION", "RUINA2-MARIONETTE"] },
            { "ACTSFROMTHEPAST-SLIME_BOSS_BOSS", ["SLIMEBOSS-PREPARE_CRUSH"]},
            { "ACTSFROMTHEPAST-GUARDIAN_BOSS", ["GUARDIAN-BODY_CRASH"]},
            { "ACTSFROMTHEPAST-HEXAGHOST_BOSS", ["HEXAGHOST-ETHER_STEP"]},
            { "ACTSFROMTHEPAST-BRONZE_AUTOMATON_BOSS", ["AUTOMATON-HYPER_BEAM_AUTOMATON"]},
            { "ACTSFROMTHEPAST-CHAMP_BOSS", ["CHAMP-MURDER_STRIKE"]},
            { "ACTSFROMTHEPAST-AWAKENED_ONE_BOSS", ["AWAKENED-MURDER"]}
        };
        if (!moddedEnemyMap.TryGetValue(encounterId.Entry, out var value)) return null;
        if (value.Count > 1) value.StableShuffle(player.PlayerRng.Rewards);
        if (value.Count == 0) return null;
        var id = new ModelId("CARD", value[0]);
        var card = ModelDb.GetByIdOrNull<CardModel>(id);
        return card;
    }
    
    
    
}