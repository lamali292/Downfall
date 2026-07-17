using Gremlins.GremlinsCode.Events;
using Gremlins.GremlinsCode.Vfx;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Screens.Overlays;
using MegaCrit.Sts2.Core.Runs;

namespace Gremlins.GremlinsCode.Core;

public static class GremlinsCmd
{
    private static readonly LocString NoGremlinSwap = new("combat_messages", "NO_GREMLIN_SWAP");

    private static async Task SwitchGremlin(PlayerChoiceContext ctx, Player player, Creature gremlin,
        GremlinSwapType gremlinSwapType)
    {
        if (player.Creature.CombatState == null) return;
        var state = GremlinsRunModel.GetState(player);
        var node = NCombatRoom.Instance?.GetCreatureNode(player.Creature);
        if (node?.Visuals is NGremlinsCreatureVisuals visuals)
            visuals.SwitchToGremlin(gremlin, state.Bench.Prepend(gremlin));
        await GremlinsHook.AfterGremlinSwap(player.Creature.CombatState, ctx, player, gremlinSwapType);
    }


    public static async Task KillGremlin(PlayerChoiceContext ctx, Player player, Creature gremlin)
    {
        var state = GremlinsRunModel.GetState(player);
        state.Kill(gremlin);
        var next = state.Active;
        if (next != null)
            await ApplySwap(ctx, player, state, next, GremlinSwapType.Death);
        var node = NCombatRoom.Instance?.GetCreatureNode(player.Creature);
        if (node?.Visuals is NGremlinsCreatureVisuals)
            NGremlinsCreatureVisuals.KillGremlin(gremlin);
    }


    public static Creature? GetCurrentGremlin(Player? player)
    {
        return player == null ? null : GremlinsRunModel.GetState(player).Active;
    }

    private static async Task<Creature?> SelectGremlin(PlayerChoiceContext ctx, Player player)
    {
        // TODO : make choice with gremlin cards instead
        var state = GremlinsRunModel.GetState(player);
        var bench = state.Bench.ToList();

        switch (bench.Count)
        {
            case 0:
                return null;
            case 1:
                return bench[0];
        }

        var choiceId = RunManager.Instance.PlayerChoiceSynchronizer.ReserveChoiceId(player);
        await ctx.SignalPlayerChoiceBegunCompatibility(player, PlayerChoiceOptions.None);

        Creature? chosen;
        if (LocalContext.IsMe(player))
        {
            var overlay = NGremlinSelectOverlay.Create(bench);
            NOverlayStack.Instance!.Push(overlay);
            overlay.ZIndex = 10;
            var slot = await overlay.AwaitSelection();
            NOverlayStack.Instance.Remove(overlay);
            chosen = bench[slot];
            RunManager.Instance.PlayerChoiceSynchronizer.SyncLocalChoice(
                player, choiceId, PlayerChoiceResult.FromIndex(slot));
        }
        else
        {
            var slot = (await RunManager.Instance.PlayerChoiceSynchronizer
                .WaitForRemoteChoice(player, choiceId)).AsIndex();
            chosen = slot < 0 ? null : bench[slot];
        }

        await ctx.SignalPlayerChoiceEnded();
        return chosen;
    }

    public static async Task SwapToSelected(PlayerChoiceContext ctx, Player player)
    {
        var gremlin = await SelectGremlin(ctx, player);
        if (gremlin == null) return;
        await Swap(ctx, player, gremlin);
    }

    public static async Task SwapToNext(PlayerChoiceContext ctx, Player player)
    {
        var state = GremlinsRunModel.GetState(player);
        if (state.Next == null) return;
        await Swap(ctx, player, state.Next);
    }

    public static async Task SwapToType<T>(PlayerChoiceContext ctx, Player player)
        where T : GremlinsMonsterModel
    {
        var target = GremlinsRunModel.GetState(player).Bench
            .FirstOrDefault(g => g.Monster is T);
        if (target == null) return;
        await Swap(ctx, player, target);
    }

    public static async Task SwapToRandom(PlayerChoiceContext ctx, Player player)
    {
        var bench = GremlinsRunModel.GetState(player).Bench.ToList();
        if (bench.Count == 0) return;
        await Swap(ctx, player, bench[player.RunState.Rng.Niche.NextInt(bench.Count)]);
    }


    public static int GetLivingGremlinCount(Player player)
    {
        var state = GremlinsRunModel.GetState(player);
        return (state.Active != null ? 1 : 0) + state.Bench.Count();
    }

    public static Creature? ResurrectRandomGremlin(Player player, int hp)
    {
        var state = GremlinsRunModel.GetState(player);
        var dead = GremlinsRunModel.StartingGremlins
            .Where(m => state.Gremlins.All(g => g.Monster?.Id != m.Id))
            .ToList();
        if (dead.Count == 0) return null;

        var model = dead[player.RunState.Rng.Niche.NextInt(dead.Count)];
        var maxHp = Math.Max(model.MaxInitialHp, hp);
        AddGremlin(player, model, hp, maxHp);
        var creature = state.Gremlins[^1];
        var node = NCombatRoom.Instance?.GetCreatureNode(player.Creature);
        if (node?.Visuals is NGremlinsCreatureVisuals visuals)
            visuals.ReviveGremlin(creature);
        return creature;
    }

    private static async Task Swap(PlayerChoiceContext ctx, Player player, Creature target)
    {
        if (!GremlinsHook.ShouldGremlinSwap(player.Creature.CombatState!, player, target))
        {
            if (LocalContext.IsMe(player)) ThinkCmd.Play(NoGremlinSwap, player.Creature, 2.0);
            return;
        }

        var state = GremlinsRunModel.GetState(player);
        // write current HP back to the outgoing gremlin before swapping
        var active = state.Active;
        if (active != null)
        {
            active.SetCurrentHpInternal(player.Creature.CurrentHp);
            active.SetMaxHpInternal(player.Creature.MaxHp);
        }

        state.SwapTo(target);
        await ApplySwap(ctx, player, state, target, GremlinSwapType.Move);
    }

    // GremlinsCmd
    public static void AddGremlin(Player player, MonsterModel model, int hp, int maxHp)
    {
        var combatState = CombatManager.Instance.DebugOnlyGetState();
        if (combatState == null) return;
        if (player.PlayerCombatState == null) return;

        var state = GremlinsRunModel.GetState(player);
        var mutable = model.ToMutable();
        var creature = combatState.CreateCreature(mutable, CombatSide.Player, null);
        mutable.SetUpForCombat();
        creature.PetOwner = player;
        creature.MaxHp = maxHp;
        creature.CurrentHp = hp;
        player.PlayerCombatState.AddPetInternal(creature);
        state.Register(creature);
        NCombatRoom.Instance!.AddCreature(creature);

        var creatureNode = NCombatRoom.Instance.GetCreatureNode(player.Creature);
        if (creatureNode?.Visuals is NGremlinsCreatureVisuals visuals)
            visuals.ArrangeGremlins(state.Gremlins);
    }

    public static async Task TriggerGremlinBonus(PlayerChoiceContext ctx, Player player)
    {
        var gremlin = GetCurrentGremlin(player);
        if (gremlin?.Monster is not GremlinsMonsterModel monster) return;
        await monster.TriggerGremlinBonus(ctx, player);
    }

    private static async Task ApplySwap(PlayerChoiceContext ctx, Player player, GremlinState state, Creature target,
        GremlinSwapType swapType)
    {
        await SwitchGremlin(ctx, player, target, swapType);
        player.Creature.SetMaxHpInternal(target.MaxHp);
        player.Creature.SetCurrentHpInternal(target.CurrentHp);
    }
}

public enum GremlinSwapType
{
    Death,
    Move
}