using Automaton.AutomatonCode.Cards.Rare;
using Awakened.AwakenedCode.Cards.Rare;
using BaseLib.Extensions;
using Champ.ChampCode.Cards.Rare;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Events;
using Collector.CollectorCode.Extensions;
using Collector.CollectorCode.Powers;
using Downfall.DownfallCode.Commands;
using Downfall.DownfallCode.Compatibility;
using Godot;
using Guardian.GuardianCode.Cards.Rare;
using Hexaghost.HexaghostCode.Cards.Rare;
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
using SlimeBoss.SlimeBossCode.Cards.Rare;

namespace Collector.CollectorCode.Core;

public class CollectorCmd
{
    
    public static AttackCommand? TorchheadAttack(AbstractModel model, CardPlay? cardplay = null)
    {
        return TorchheadAttack(model.Player, model.DynamicVars.TorchheadDamage.IntValue, model as CardModel, cardplay);
    }
    
    public static AttackCommand? TorchheadAttack(Player player, int damage, CardModel? card = null, CardPlay? cardplay = null)
    {
        var shouldTargetAll = CollectorHook.ShouldTorchheadTargetAll(player, out _);
        if (player.Creature.CombatState == null || player.Torchhead?.Monster is not TorchheadMonsterModel torchhead)
        {
            return null;
        }
        var attack = DamageCmd.Attack(damage)
            .FromTorchhead(torchhead, card, cardplay)
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
        var torchhead = await DownfallCmd.Summon<TorchheadMonsterModel, TorchheadPower>(ctx, summoner, hp, source);
        RefreshTorchheadIntent(torchhead);
        RefreshTorchheadScale(torchhead);
        return torchhead;
    }

    /// <summary>
    /// Torchhead never runs a real monster turn (it's summoned mid-combat, and its attack fires
    /// from TorchheadPower.AfterSideTurnEnd instead), so its move is never rolled by the normal
    /// enemy turn loop and its intent icon would stay blank. Call this whenever the pet is summoned
    /// or its damage may have changed, so the shown value stays accurate.
    /// </summary>
    public static void RefreshTorchheadIntent(Creature torchhead)
    {
        var combatState = torchhead.CombatState;
        if (combatState == null) return;
        torchhead.PrepareForNextTurn(combatState.Players.Select(p => p.Creature));
    }

    private const float TorchheadMinScale = 1f;
    private const float TorchheadMaxScale = 1.75f;
    private const float TorchheadScaleCapHp = 80f;

    /// <summary>
    /// Grows Torchhead's visuals with its Max HP, same idea as Osty (NCreature.OstyScaleToSize) -
    /// that method is hardcoded to Osty's own scale/offset constants though, so this mirrors just
    /// the size half via the generic NCreature.ScaleTo, with Torchhead's own range/cap.
    /// </summary>
    public static void RefreshTorchheadScale(Creature torchhead)
    {
        var t = Mathf.Clamp(torchhead.MaxHp / TorchheadScaleCapHp, 0f, 1f);
        var scale = Mathf.Lerp(TorchheadMinScale, TorchheadMaxScale, t);
        NCombatRoom.Instance?.GetCreatureNode(torchhead)?.ScaleTo(scale, 0.75);
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
            { "ACTSFROMTHEPAST-SLIME_BOSS_BOSS", [ModelDb.Card<PrepareCrush>().Id.Entry]},
            { "ACTSFROMTHEPAST-GUARDIAN_BOSS", [ModelDb.Card<BodyCrash>().Id.Entry]},
            { "ACTSFROMTHEPAST-HEXAGHOST_BOSS", [ModelDb.Card<EtherStep>().Id.Entry]},
            { "ACTSFROMTHEPAST-BRONZE_AUTOMATON_BOSS", [ModelDb.Card<HyperBeamAutomaton>().Id.Entry]},
            { "ACTSFROMTHEPAST-CHAMP_BOSS", [ModelDb.Card<MurderStrike>().Id.Entry]},
            { "ACTSFROMTHEPAST-AWAKENED_ONE_BOSS", [ModelDb.Card<Murder>().Id.Entry]}
        };
        if (!moddedEnemyMap.TryGetValue(encounterId.Entry, out var value)) return null;
        if (value.Count > 1) value.StableShuffle(player.PlayerRng.Rewards);
        if (value.Count == 0) return null;
        var id = new ModelId("CARD", value[0]);
        var card = ModelDb.GetByIdOrNull<CardModel>(id);
        return card;
    }
    
    
    
}