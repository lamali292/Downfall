using BaseLib.Extensions;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Events;
using Collector.CollectorCode.Extensions;
using Collector.CollectorCode.Powers;
using Downfall.DownfallCode.Abstract;
using Downfall.DownfallCode.Commands;
using Downfall.DownfallCode.Compatibility;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace Collector.CollectorCode.Core;

public class CollectorCmd
{
    
    public static AttackCommand? TorchheadAttack(AbstractModel card)
    {
        var player = card.Player;
        var damage = card.DynamicVars.TorchheadDamage.IntValue;
        return TorchheadAttack(player, damage);
    }
    
    public static AttackCommand? TorchheadAttack(Player player, int damage)
    {
        var shouldTargetAll = CollectorHook.ShouldTorchheadTargetAll(player, out _);
        if (player.Creature.CombatState == null || player.Torchhead?.Monster is not TorchheadMonsterModel torchhead)
        {
            return null;
        }
        var attack = DamageCmd.Attack(damage)
            .FromTorchhead(torchhead)
            .WithHitFx("vfx/vfx_attack_blunt", tmpSfx: "blunt_attack.mp3");
        if (shouldTargetAll)
        {
            return attack.TargetingAllOpponents(player.Creature.CombatState);
        }

        var target = player.Creature.CombatState?.HittableEnemies.OrderBy(e => e.CurrentHp).FirstOrDefault();
        return target == null ? null: attack.Targeting(target);
    }
    
    
    private static async Task PyreCards(PlayerChoiceContext ctx, CardModel card, IEnumerable<CardModel> pyred)
    {
        if (card.CombatState == null) return;
        foreach (var c in pyred)
        {
            if (CollectorHook.ShouldExhaustPyred(card, c))
            {
                await CardCmdCompatibility.Exhaust(ctx, c);
            }
            await CollectorHook.AfterCardPyred(card.CombatState, ctx, card, c);
            await Cmd.Wait(0.1f);
        }
    }

    public static async Task<CardModel?> Pyre(PlayerChoiceContext ctx, CardModel card)
    {
        var prefs = new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1, 1);
        var pyred = (await CardSelectCmd.FromHand(ctx, card.Owner, prefs, e => e != card, card)).FirstOrDefault();
        if (pyred == null || card.CombatState == null) return pyred;
        await PyreCards(ctx, card, [pyred]);
        return pyred;
    }

    public static async Task<IReadOnlyList<CardModel>> MegaPyre(PlayerChoiceContext ctx, CardModel card)
    {
        if (card.CombatState == null) return [];
        var cards = card.Owner.Hand.ToList();
        await PyreCards(ctx, card, cards);
        return cards;
    }


    
    
    public static Task<Creature> Kindle(
        PlayerChoiceContext ctx,
        AbstractModel source)
    {
        return Kindle(ctx, source.Player, source);
    }

    public static Task<Creature> Kindle(
        PlayerChoiceContext ctx,
        Player summoner,
        AbstractModel source)
    {
        return Kindle(ctx, summoner, source.DynamicVars.Kindle.IntValue, source);
    }
    
    
    public static async Task<Creature> Kindle(
        PlayerChoiceContext ctx,
        Player summoner,
        int hp,
        AbstractModel? source)
    {
        if (summoner is not { Osty: not null, Torchhead: null }) //User has an Osty already! (But does not have a Torchhead).
            return await DownfallCmd.Summon<TorchheadMonsterModel, TorchheadPower>(ctx, summoner, hp,
                source); //No Osty, summon on Torchhead instead.
        await CreatureCmd.TriggerAnim(summoner.Creature, Necrobinder.GetSummonAnimIfApplicable(summoner.Character), Necrobinder.GetSummonDelayIfApplicable(summoner.Character));
        await OstyCmd.Summon(ctx, summoner, hp, source);
        return await DownfallCmd.Summon<TorchheadMonsterModel, TorchheadPower>(ctx, summoner, hp, source);//No Osty, summon on Torchhead instead.
    }

    public static Task GainReserve(AbstractModel card)
    {
        return GainReserve(card.Player, card.DynamicVars.Reserve.IntValue);
    }
    
    public static Task GainReserve(Player player, int amount)
    {
        player.PlayerCombatState?.Reserve += amount;
        return Task.CompletedTask;
    }
    
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
        var model = pool.FirstOrDefault(c => c is ICollectible g && g.GetEncounterModel().Id == encounterId);
        // fallback to other mods
        model ??= GetCardForModdedEnemy(encounterId);
        // final fallback. pick random elite or boss with the same act number.
        if (model is null)
        {
            var actNumber = room.Act.ActNumber();
            model = player.RunState.Rng.Niche.NextItem(pool
                .Where(c => c is ICollectible g && (g.Act()?.ActNumber() ?? -1) == actNumber && g.RoomType() == room.RoomType));
            if (model is null)
                return false;
        }

        var card = player.RunState.CreateCard(model, player);
        action?.Invoke(card);
        var result = new CardCreationResult(card);
        result.ModifyCard(card, relic);
        cardRewardOptions.Add(result);
        return true;
    }

    private static CardModel? GetCardForModdedEnemy(ModelId encounterId)
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
        if (!moddedEnemyMap.TryGetValue(encounterId.Entry, out var value)) return null;
        var id = new ModelId("CARD", value);
        var card = ModelDb.GetById<CardModel>(id);
        return card;
    }
    
    
}