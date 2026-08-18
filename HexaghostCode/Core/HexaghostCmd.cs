using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using Godot;
using Hexaghost.HexaghostCode.CustomEnums;
using Hexaghost.HexaghostCode.Events;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace Hexaghost.HexaghostCode.Core;

public static class HexaghostCmd
{
    public static GhostflameModel[] GetWheel(Player player)
    {
        return HexaghostModel.Wheel.Get(player) ?? [];
    }
    
    /// <summary>
    /// All Afterlife-keyworded cards available to the player. Hexaghost players draw only from
    /// their own pool; other characters draw Afterlife cards from every pool.
    /// </summary>
    public static IEnumerable<CardModel> GetAfterlifeCards(Player player, int amount) =>
        DownfallCardCmd.GetSpecificCards<Hexaghost>(player, c => c.Keywords.Contains(HexaghostKeyword.Afterlife), amount);
    

    
    public static Task SoulburnEffect(Creature? creature, float scale = 0.8f, bool silent = false)
    {
        if(creature == null)   return Task.CompletedTask;
        var child = NGroundFireVfx.Create(creature, VfxColor.Green);
        if (child == null)
            return Task.CompletedTask;
        if (!silent)
            SfxCmd.Play("event:/sfx/characters/attack_fire");
        child.Scale = Vector2.One * scale;
        var instance = NCombatRoom.Instance;
        instance?.CombatVfxContainer.AddChildSafely(child);
        return Task.CompletedTask;
    }
    
    public static void ActivateGhostwheel(Player player)
    {
        HexaghostModel.Active.Set(player, true);
    }

    public static bool IsGhostwheelActivated(Player player)
    {
        return HexaghostModel.Active.Get(player);
    }

    public static int GetCurrentIndex(Player player)
    {
        return HexaghostModel.CurrentIndex.Get(player);
    }

    public static GhostflameModel GetCurrentFlame(Player player)
    {
        return GetWheel(player)[GetCurrentIndex(player)];
    }


    public static T? GetFlameOfType<T>(Player player) where T : GhostflameModel
    {
        return GetWheel(player).OfType<T>().FirstOrDefault();
    }

    public static int GetIgnitedCount(Player player)
    {
        return GetWheel(player).Count(f => f.IsIgnited);
    }

    public static bool AllIgnited(Player player)
    {
        return GetWheel(player).All(f => f.IsIgnited);
    }


    private static int GetPreviousIndex(Player player)
    {
        var wheel = GetWheel(player);
        return (GetCurrentIndex(player) + wheel.Length - 1) % wheel.Length;
    }

    private static int GetNextIndex(Player player)
    {
        var wheel = GetWheel(player);
        return (GetCurrentIndex(player) + 1) % wheel.Length;
    }

    public static async Task Advance(PlayerChoiceContext ctx, Player player, AbstractModel? source, bool silent = false,
        bool autoAdvance = false)
    {
        SfxCmd.Play("event:/sfx/characters/hexaghost-hexaghost/advance");
        await MoveTo(player, GetNextIndex(player));
        if (!autoAdvance)
            await HexaghostHook.AfterWheelAdvance(player.Creature.CombatState!, ctx, player, source,
                GetCurrentFlame(player),
                GetCurrentIndex(player), silent);
    }

    public static async Task Retract(PlayerChoiceContext ctx, Player player, AbstractModel? source, bool silent = false)
    {
        SfxCmd.Play("event:/sfx/characters/hexaghost-hexaghost/retract");
        await MoveTo(player, GetPreviousIndex(player));
        await HexaghostHook.AfterWheelRetract(player.Creature.CombatState!, ctx, player, source,
            GetCurrentFlame(player),
            GetCurrentIndex(player), silent);
    }

    public static async Task MoveToRandom(PlayerChoiceContext ctx, Player player, bool silent = false)
    {
        var wheel = GetWheel(player);
        var current = GetCurrentIndex(player);
        var rng = player.RunState.Rng.Niche;
        var candidates = Enumerable.Range(0, wheel.Length).Where(i => i != current).ToArray();
        var randomIndex = rng.NextItem(candidates);
        await MoveTo(player, randomIndex, silent);
    }

    public static Task ReplaceCurrentWithRandom(Player player)
    {
        var wheel = GetWheel(player);
        var currentIdx = GetCurrentIndex(player);
        var rng = player.RunState.Rng.Niche;

        var current = wheel[currentIdx];
        var isOffclass = current.IsOffclass;
        var currentType = current.GetType();
        var candidates = HexaghostModelDb.AllGhostflames.Where(f => f.GetType() != currentType && f.IsOffclass == isOffclass).ToArray();
        var randomFlame = rng.NextItem(candidates);

        if (randomFlame == null) return Task.CompletedTask;
        wheel[currentIdx] = randomFlame.ToMutable(player);
        Refresh(player);
        return Task.CompletedTask;
    }


    private static Task MoveTo(Player player, int index, bool silent = false)
    {
        if (player.PlayerCombatState == null) return Task.CompletedTask;
        ActivateGhostwheel(player);
        HexaghostModel.CurrentIndex[player] = index;
        var flame = GetCurrentFlame(player);
        flame.Extinguish();
        flame.UpdateVisuals();
        if (silent) return Task.CompletedTask;
        Refresh(player);
        return Task.CompletedTask;
    }

    public static bool IsIgnited(Player player)
    {
        return GetCurrentFlame(player).IsIgnited;
    }

    public static bool IsPreviousIgnited(Player player)
    {
        return GetWheel(player)[GetPreviousIndex(player)].IsIgnited;
    }

    public static bool IsNextIgnited(Player player)
    {
        return GetWheel(player)[GetNextIndex(player)].IsIgnited;
    }

    public static Task IgnitePrevious(PlayerChoiceContext ctx, Player player)
    {
        return IgniteAt(ctx, player, GetPreviousIndex(player));
    }

    public static Task IgniteNext(PlayerChoiceContext ctx, Player player)
    {
        return IgniteAt(ctx, player, GetNextIndex(player));
    }

    public static Task Ignite(PlayerChoiceContext ctx, Player player)
    {
        return IgniteAt(ctx, player, GetCurrentIndex(player));
    }

    public static async Task IgniteAt(PlayerChoiceContext ctx, Player player, int index)
    {
        ActivateGhostwheel(player);
        await Cmd.Wait(0.05f);
        var flame = GetWheel(player)[index];
        if (!flame.IsIgnited)
            flame.IsIgnited = true;

        var allIgnited = AllIgnited(player);
        flame.SetIgniteProgress();
        Refresh(player);
        await flame.OnIgnite(ctx);
        await HexaghostHook.AfterGhostwheelIgnited(player.Creature.CombatState!, ctx, player, flame, index);
        await Cmd.Wait(0.05f);
        if (allIgnited)
            await HexaghostHook.AfterGhostwheelAllIgnited(player.Creature.CombatState!, ctx, player, flame, index);
    }


    public static async Task IgniteAll(PlayerChoiceContext ctx, Player player)
    {
        var wheel = GetWheel(player);
        for (var i = 0; i < wheel.Length; i++) await IgniteAt(ctx, player, i);
    }

    public static Task ExtinguishAllExceptThis(PlayerChoiceContext ctx, Player player, GhostflameModel model)
    {
        foreach (var f in GetWheel(player).Where(e => e != model))
            f.Extinguish();
        Refresh(player);
        return Task.CompletedTask;
    }


    public static Task Extinguish(Player player, bool silent = false)
    {
        GetCurrentFlame(player).Extinguish();
        if (silent) return Task.CompletedTask;
        Refresh(player);
        return Task.CompletedTask;
    }

    public static void Refresh(Player player)
    {
        if (!IsGhostwheelActivated(player)) return;
        HexaghostVisualsBridge.Refresh(player);
    }

    public static Task<int> ResetWheel(Player player)
    {
        Cmd.Wait(0.1f);
        var a = GetWheel(player).Count(flame => flame.Extinguish());
        Cmd.Wait(0.1f);
        HexaghostModel.ResetWheel(player);
        Cmd.Wait(0.1f);
        Refresh(player);
        return Task.FromResult(a);
    }

    public static void SetCurrentGhostflame(Player player, GhostflameModel ghostflame)
    {
        ghostflame.AssertCanonical();
        ActivateGhostwheel(player);
        GetWheel(player)[GetCurrentIndex(player)] = ghostflame.ToMutable(player);
        Refresh(player);
    }
    
    public static AttackCommand AfterlifeAttack(CardModel card, CardPlay? cardPlay)
    {
        AttackCommand a;
        if (card.DynamicVars.ContainsKey("CalculatedDamage"))
            a = DamageCmd.Attack(card.DynamicVars.CalculatedDamage);
        else if (card.DynamicVars.ContainsKey("Damage"))
            a = DamageCmd.Attack(card.DynamicVars.Damage.BaseValue);
        else 
            throw new Exception($"Card {card.Title} does not have a damage variable supported by CommonActions.CardAttack");
        a = a.FromCardCompatibility(card, cardPlay);
        if (cardPlay?.Target != null)
        {
            return a.Targeting(cardPlay.Target);
        }
        if (card.CombatState != null)
        {
            return card.TargetType == TargetType.AllEnemies ? a.TargetingAllOpponents(card.CombatState) : a.TargetingRandomOpponents(card.CombatState);
        }
        throw new InvalidOperationException("Afterlife attack failed!");
    }
}