using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using SlimeBoss.SlimeBossCode.Cards.Token;
using SlimeBoss.SlimeBossCode.Events;
using SlimeBoss.SlimeBossCode.Extensions;
using SlimeBoss.SlimeBossCode.Interfaces;
using SlimeBoss.SlimeBossCode.Powers;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Core;

public static class SlimeBossCmd
{
    public static IEnumerable<SlimeModel> GetSlimes(Player player)
    {
        return player.Slimes.Select(e => e.Monster).OfType<SlimeModel>();
    }
    
    private static SlimeModel? GetFirstSlime(Player player)
    {
        return GetSlimes(player).LastOrDefault();
    }

    /// <summary>Potency granted instead of duplicating an already-summoned slime type.</summary>
    private const int DuplicateSplitPotency = 2;

    
    /// <summary>
    /// "Consume - if the enemy has Weak, remove a stack of Weak and perform an additional effect." Always
    /// attempted explicitly by the card itself (like Schlurp) rather than gated behind an attack landing -
    /// there is no separate Goop resource anymore, Consume keys directly off the target's Weak stacks.
    /// </summary>
    public static async Task<bool> Consume(PlayerChoiceContext ctx, CardModel card, CardPlay? cardPlay)
    {
        if (cardPlay == null) return false;
        // card.GetTargets() only resolves AoE target types; single-target cards (AnyEnemy etc.) need the
        // actually-clicked target from CardPlay - MyGetTargets handles both uniformly (see the
        // DuplicatedFormDoublesAoeCardsTargetingEnemies regression test for the AoE-side version of this).
        var targets = card.MyGetTargets(cardPlay.Target);
        List<Creature> consumed = [];
        foreach (var target in targets)
        {
            var weak = target.GetPower<WeakPower>();
            if (weak is not { Amount: > 0 }) continue;

            await PowerCmd.ModifyAmount(ctx, weak, -1, null, card);
            consumed.Add(target);
            if (card is IHasConsumeEffect effect) await effect.ConsumeEffect(ctx, cardPlay, target);
            if (target.CombatState != null)
                await SlimeBossHook.AfterConsumeEffect(target.CombatState, ctx, target, card.Owner.Creature);
        }
        return consumed.Any();
    }
    


    private static async Task RunCommand(PlayerChoiceContext ctx, Player player, SlimeModel slime, CardModel? source)
    {
        await slime.Command(ctx);
        if (player.Creature.CombatState == null) return;
        await SlimeBossHook.AfterCommand(player.Creature.CombatState, ctx, player, slime, source);
    }

    private static async Task CommandInternal(PlayerChoiceContext ctx, Player player,
        CardModel? source, CommandType commandType = CommandType.First)
    {
        switch (commandType)
        {
            case CommandType.First:
                var slime = GetFirstSlime(player);
                if (slime == null) return;
                await RunCommand(ctx, player, slime, source);
                break;
            case CommandType.All:
                await GetSlimes(player).Reverse().ForEachAsync(s => RunCommand(ctx, player, s, source));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(commandType), commandType, null);
        }
    }

    public static async Task Command(PlayerChoiceContext ctx, Player player, int amount,
        CardModel? cardSource = null, CommandType commandType = CommandType.First)
    {
        for (var i = 0; i < amount; i++) await CommandInternal(ctx, player, cardSource, commandType);
    }
    

    /// <summary>
    /// Commands a specific slime type (e.g. "Command Bruiser Slime"). If the player has not split into that
    /// slime yet, they Split into it first before it acts.
    /// </summary>
    public static async Task Command<T>(PlayerChoiceContext ctx, Player player, int amount,
        CardModel? cardSource = null) where T : SlimeModel
    {
        for (var i = 0; i < amount; i++)
        {
            var slime = GetSlimes(player).OfType<T>().FirstOrDefault();
            if (slime == null)
            {
                await Split<T>(ctx, player);
                slime = GetSlimes(player).OfType<T>().FirstOrDefault();
            }

            if (slime != null) await RunCommand(ctx, player, slime, cardSource);
        }
    }

    public static Task Command<T>(PlayerChoiceContext ctx, CardModel card)
        where T : SlimeModel
    {
        return Command<T>(ctx, card.Owner, card.DynamicVars["Command"].IntValue, card);
    }
    
    public static Task CommandAll(PlayerChoiceContext ctx, Player player, int amount = 1,
        CardModel? cardSource = null)
    {
        return Command(ctx, player, amount, cardSource, CommandType.All);
    }
    

    public static Task<Creature?> Split<T>(PlayerChoiceContext ctx, Player player) where T : SlimeModel
    {
        return Split(ctx, player, SlimeBossModelDb.Slime<T>());
    }

    /// <summary>
    /// Splits into the given slime type. If the player already has one, grants it Potency instead
    /// of spawning a duplicate. Used by any "Split into X" effect, including runtime-chosen slime
    /// types (e.g. Unison, Split Specialist) that don't have a compile-time type parameter.
    /// </summary>
    public static async Task<Creature?> Split(PlayerChoiceContext ctx, Player player, SlimeModel slimeModel)
    {
        var existing = player.Creature.Pets.FirstOrDefault(e => e.Monster?.GetType() == slimeModel.GetType());
        if (existing == null)
        {
            return await SpawnSlime(ctx, player, slimeModel);
        }
        await PowerCmd.Apply<PotencyPower>(ctx, existing, DuplicateSplitPotency, player.Creature, null);
        return existing;
    }

    /// <summary>
    /// Splits into a slime of type T even if one already exists (bypasses the duplicate-grants-Potency rule).
    /// Used by cards that intentionally spawn multiple of the same slime at once (e.g. Darkling Duo).
    /// </summary>
    public static Task SplitForced<T>(PlayerChoiceContext ctx, Player player) where T : SlimeModel
    {
        return SpawnSlime(ctx, player, SlimeBossModelDb.Slime<T>());
    }

    private static async Task<Creature?> SpawnSlime(PlayerChoiceContext ctx, Player player, SlimeModel slimeModel)
    {
        var slime = await AddSlime(player, slimeModel);
        if (player.Creature.CombatState == null) return slime;
        await SlimeBossHook.AfterSplit(player.Creature.CombatState, ctx, player, slimeModel);
        return slime;
    }


    public static async Task SplitSpecialist(PlayerChoiceContext ctx, Player player)
    {
        var combatState = player.Creature.CombatState;
        if (combatState == null) return;
        var slimeCards = SlimeBossModelDb.AllSpecialistSlimes
            .TakeRandom(3, player.RunState.Rng.CombatCardGeneration)
            .Select(SlimeBossModelDb.GetCardForSlime).Select(e => combatState.CreateCard(e, player)).ToList();
        var card = await CardSelectCmd.FromChooseACardScreen(ctx, slimeCards, player);
        if (card is not ISlimeCard slimeCard) return;
        var slime = slimeCard.SlimeModel;
        await Split(ctx, player, slime);
    }
    
     public static async Task<Creature?> AddSlime(Player player, SlimeModel slimeModel)
    {
        var pet = player.Creature.CombatState?.CreateCreature(slimeModel.ToMutable(), player.Creature.Side, null);
        if (pet == null) return null;
        await PlayerCmd.AddPet(pet, player);
        Callable.From(() => RearrangeSlimeOrbRow(player)).CallDeferred();
        return pet;
    }

    

    private static void RearrangeSlimeOrbRow(Player player)
    {
        var playerNode = NCombatRoom.Instance?.GetCreatureNode(player.Creature);
        if (playerNode == null) return;

        var slimes = player.Creature.Pets.Where(e => e.Monster is SlimeModel).ToList();
        var totalSlimes = slimes.Count;
        if (totalSlimes == 0) return;

        const float maxSpacing = 300f;

        var startPoint = new Vector2(300f, -50f);
        var endPoint = new Vector2(-150f, 200f);
        var apexPoint = new Vector2(400f, 150f);

        var chordLength = startPoint.DistanceTo(apexPoint) + apexPoint.DistanceTo(endPoint);
        var tDeltaPerSpacing = maxSpacing / chordLength;
        var totalRequestedTSpan = (totalSlimes - 1) * tDeltaPerSpacing;

        const float tStart = 0.0f;
        var tEnd = totalRequestedTSpan;
        if (totalRequestedTSpan > 1.0f && totalSlimes > 1) tEnd = 1.0f;

        for (var i = 0; i < totalSlimes; i++)
        {
            var activePet = slimes[i];
            var slimeNode = NCombatRoom.Instance?.GetCreatureNode(activePet);
            slimeNode?.ToggleIsInteractable(true);
            if (slimeNode == null) continue;
            HideHealthBar(slimeNode);
            var layoutIndex = totalSlimes - 1 - i;

            var t = totalSlimes == 1 ? 0.0f : Mathf.Lerp(tStart, tEnd, (float)layoutIndex / (totalSlimes - 1));
            t = Mathf.Clamp(t, 0.0f, 1.0f);

            var relativeOffset = CalculateQuadraticBezier(startPoint, apexPoint, endPoint, t);

            if (player.Creature.Side == CombatSide.Enemy) relativeOffset.X = -relativeOffset.X;

            var targetGlobalPos = playerNode.GlobalPosition + relativeOffset;

            // convert the global target into the slime node's parent-local space,
            // since Node2D.Position is relative to the parent
            var targetLocalPos = slimeNode.GetParent() is Node2D parent
                ? parent.ToLocal(targetGlobalPos)
                : targetGlobalPos;

            var currentPos = slimeNode.Position;

            if (!slimeNode.HasMeta("layout_tween"))
            {
                // first layout: snap instantly, no tween
                slimeNode.Position = targetLocalPos;
                slimeNode.UpdateBounds(slimeNode.Visuals);
                currentPos = targetLocalPos;
            }

            if (!slimeNode.HasMeta("layout_tween"))
            {
                slimeNode.GlobalPosition = targetGlobalPos;
                slimeNode.UpdateBounds(slimeNode.Visuals);
            }

            var layoutTween = slimeNode.CreateTween();
            slimeNode.SetMeta("layout_tween", layoutTween);
            layoutTween.TweenProperty(slimeNode, "global_position", targetGlobalPos, 0.35f)
                .From(slimeNode.GlobalPosition)
                .SetEase(Tween.EaseType.Out)
                .SetTrans(Tween.TransitionType.Cubic);

            layoutTween.Parallel().TweenCallback(Callable.From(() => slimeNode.UpdateBounds(slimeNode.Visuals)));
        }
    }

    private static void HideHealthBar(NCreature slimeNode)
    {
        slimeNode._stateDisplay._healthBar.Visible = false;
    }

    private static Vector2 CalculateQuadraticBezier(Vector2 p0, Vector2 p1, Vector2 p2, float t)
    {
        var u = 1f - t;
        var tt = t * t;
        var uu = u * u;

        var point = uu * p0 + 2f * u * t * p1 + tt * p2;
        return point;
    }
}

public enum CommandType
{
    First,
    All
}