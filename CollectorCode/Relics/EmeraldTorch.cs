using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace Collector.CollectorCode.Relics;

[Pool(typeof(CollectorRelicPool))]
public class EmeraldTorch : CollectorRelicModel
{
    public EmeraldTorch() : base(RelicRarity.Starter)
    {
        WithKindle(4);
    }
    
    public override RelicModel GetUpgradeReplacement()
    {
        return ModelDb.Relic<PrismaticTorch>();
    }
    
    public override async Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext ctx,
        ICombatState combatState)
    {
        if (player != Owner || Owner.PlayerCombatState is not { TurnNumber: 1 }) return;
        await CollectorCmd.Kindle(ctx, this);
        Flash();
    }
    
    /*
    public override Task AfterCombatEnd(CombatRoom room)
    {
        if (room.RoomType is not (RoomType.Elite or RoomType.Boss)) return Task.CompletedTask;
        var existsCard = ModelDb.CardPool<CollectibleCardPool>().AllCards.Any(c => c is ICollectible col && col.GetEncounterModel().Id == room.Encounter.Id);
        if (!existsCard) return Task.CompletedTask;
        foreach (var player in room.CombatState.Players.Where(p => p.Character is Core.Collector))
        {
            room.AddExtraReward(player, new CollectibleReward(room.Encounter.Id, player, false));
        }
        return Task.CompletedTask;
    }
    */
    
    public override bool TryModifyCardRewardOptions(Player player, List<CardCreationResult> cardRewardOptions, CardCreationOptions creationOptions)
    {
        if (Owner != player
            || creationOptions.Source != CardCreationSource.Encounter
            || !creationOptions.Flags.HasFlag(CardCreationFlags.IsCardReward)
            || !creationOptions.Flags.HasFlag(CardCreationFlags.IsFromCombat))
            return false;
        
        var room = player.RunState.CurrentRoom as CombatRoom;
        if (room?.RoomType is not (RoomType.Elite or RoomType.Boss))
            return false;

        var encounterId = room.Encounter.Id;

        var model = ModelDb.CardPool<CollectibleCardPool>().AllCards
            .FirstOrDefault(c => c is ICollectible g && g.GetEncounterModel().Id == encounterId);
        if (model is null)
        {
            model = GetCardForModdedEnemy(encounterId);
            if (model is null)
            {
                return false;
            }
        }

        var card = player.RunState.CreateCard(model, player);
        var result = new CardCreationResult(card);
        result.ModifyCard(card, this);
        cardRewardOptions.Add(result);
        return true;
        
    }

    /*
    public override Task AfterModifyingCardRewardOptions()
    {
        Flash();
        return Task.CompletedTask;
    }*/
    
    public static CardModel? GetCardForModdedEnemy(ModelId encounterId)
    {
        var moddedEnemyMap = new Dictionary<string, string>
        {
            { "RUINA2-ALRIUNE_ELITE", "RUINA2-FAINT_AROMA" },
            { "RUINA2-HELPERS_ELITE", "RUINA2-GRINDER" },
            { "RUINA2-LAETITIA_ELITE", "RUINA2-LAETITIA" },
            { "RUINA2-FAIRY_BOSS", "RUINA2-WINGBEAT" },
            { "RUINA2-NOTHING_DER_BOSS", "RUINA2-MAGIC_BULLET" },
            { "RUINA2-BLACK_SWAN_BOSS", "RUINA2-BLACK_SWAN" },
            { "RUINA2-ORCHESTRA_BOSS", "RUINA2-DA_CAPO" },
            { "RUINA2-MOUNTAIN_ELITE", "RUINA2-SMILE" },
            { "RUINA2-WRATH_ELITE", "RUINA2-BLIND_RAGE" },
            { "RUINA2-ROAD_HOME_ELITE", "RUINA2-HOMING_INSTINCT" },
            { "RUINA2-RED_WOLF_BOSS", "RUINA2-CRIMSON_SCAR" },
            { "RUINA2-JESTER_BOSS", "RUINA2-NIHIL" },
            { "RUINA2-OZ_BOSS", "RUINA2-FALSE_THRONE" },
            { "RUINA2-BIG_BIRD_ELITE", "RUINA2-LAMP" },
            { "RUINA2-BLUE_STAR_ELITE", "RUINA2-SOUND_OF_A_STAR" },
            { "RUINA2-SNOW_QUEEN_ELITE", "RUINA2-FROST_SPLINTER" },
            { "RUINA2-TWILIGHT_BOSS", "RUINA2-APOCALYPSE" },
            { "RUINA2-WHITE_NIGHT_BOSS", "RUINA2-PARADISE_LOST" },
            { "RUINA2-SILENT_GIRL_BOSS", "RUINA2-REMORSE" },
        };
        if (moddedEnemyMap.ContainsKey(encounterId.Entry))
        {
            ModelId id = new ModelId("CARD", moddedEnemyMap[encounterId.Entry]);
            var card = ModelDb.GetById<CardModel>(id);
            return card;
        }
        return null;
    }
}