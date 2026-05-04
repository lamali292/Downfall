// ChampModel.cs

using System.Runtime.CompilerServices;
using BaseLib.Abstracts;
using BaseLib.Utils;
using Champ.ChampCode.Events;
using Champ.ChampCode.Stance;
using Champ.ChampCode.Vfx;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Champ.ChampCode.Core;

public class ChampModel() : CustomSingletonModel(true, true)
{
    private static readonly SpireField<Player, ChampStanceModel> ActiveStance =
        new(ChampModelDb.ChampStance<ChampNoStance>);

    private static readonly ConditionalWeakTable<Player, NChampStanceDisplay> StanceDisplays = new();

    public static T GetStanceAs<T>(Player player) where T : ChampStanceModel
    {
        return (ActiveStance[player] as T)!;
    }

    public static ChampStanceModel GetStanceModel(Player player)
    {
        return ActiveStance[player] ?? ChampModelDb.ChampStance<ChampNoStance>();
    }

    public static bool IsInStance<T>(Player player) where T : ChampStanceModel
    {
        return ActiveStance[player] is T;
    }

    private static NChampStanceDisplay? GetDisplay(Player player)
    {
        return StanceDisplays.TryGetValue(player, out var d) ? d : null;
    }

    private static void RegisterDisplay(Player player, NChampStanceDisplay display)
    {
        StanceDisplays.AddOrUpdate(player, display);
    }

    public static void RefreshDisplay(Player player)
    {
        GetDisplay(player)?.Refresh();
    }

    public static async Task SetStance<T>(PlayerChoiceContext ctx, Player player) where T : ChampStanceModel
    {
        await SetStance(ctx, player, ChampModelDb.ChampStance<T>());
    }

    private static async Task SetStance(PlayerChoiceContext ctx, Player player, ChampStanceModel newCanonical)
    {
        var current = ActiveStance[player];
        if (current?.GetType() == newCanonical.GetType()) return;

        if (current != null)
            await current.OnExit(ctx);

        var mutable = newCanonical.ToMutable(player);
        ActiveStance[player] = mutable;
        await mutable.OnEnter(ctx);

        TriggerStanceAnimation(player);
        await ChampHook.OnChampStanceChange(player.Creature.CombatState!, ctx, player, current!,
            ActiveStance[player]!);
        RefreshStanceDisplay(player, newCanonical);
    }

    public override async Task BeforeCombatStart()
    {
        var state = CombatManager.Instance.DebugOnlyGetState();
        if (state == null) return;
        foreach (var player in state.Players)
            ActiveStance[player] = ChampModelDb.ChampStance<ChampNoStance>();
    }


    /*
    private static void TriggerStanceAnimation(Player player)
    {
        Callable.From(() =>
        {
            var creatureNode = NCombatRoom.Instance?.GetCreatureNode(player.Creature);
            var animState = creatureNode?.SpineAnimation.GetAnimationState();
            if (animState == null) return;

            var trigger = animState.GetCurrent(0).GetAnimation().GetName() switch
            {
                "Idle" or "IdleBerserker" or "IdleDefensive" or "IdleUltimate" or "IdleGladiator" => "Idle",
                "Attack" => "Attack",
                _ => null
            };

            if (trigger == null) return;
            creatureNode?.SetAnimationTrigger(trigger);
            animState.GetCurrent(0).SetMixDuration(0.3f);
        }).CallDeferred();
    }*/
    
    private static void TriggerStanceAnimation(Player player)
    {
        Callable.From(() =>
        {
            var creatureNode = NCombatRoom.Instance?.GetCreatureNode(player.Creature);
            if (creatureNode?.Visuals is not NChampCreatureVisuals champVisuals) return;

            champVisuals.CurrentStance = ActiveStance[player] switch
            {
                ChampBerserkerStance => NChampCreatureVisuals.Stance.Berserker,
                ChampDefensiveStance => NChampCreatureVisuals.Stance.Defensive,
                ChampUltimateStance => NChampCreatureVisuals.Stance.Ultimate,
                _                    => NChampCreatureVisuals.Stance.Normal
            };

            champVisuals.OnAnimationTrigger("Idle");
        }).CallDeferred();
    }

    private static void RemoveDisplay(Player player)
    {
        StanceDisplays.Remove(player);
    }

    private static void RefreshStanceDisplay(Player player, ChampStanceModel newCanonical)
    {
        Callable.From(() =>
        {
            var existing = GetDisplay(player);

            if (newCanonical is ChampNoStance)
            {
                if (existing != null && GodotObject.IsInstanceValid(existing))
                    existing.QueueFree();
                RemoveDisplay(player);
                return;
            }

            if (existing == null || !GodotObject.IsInstanceValid(existing))
            {
                var display = NChampStanceDisplay.Show(player);
                if (display != null) RegisterDisplay(player, display);
            }
            else
            {
                existing.Refresh();
            }
        }).CallDeferred();
    }
}