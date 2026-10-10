using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using SlimeBoss.SlimeBossCode.Events;
using SlimeBoss.SlimeBossCode.Extensions;
using SlimeBoss.SlimeBossCode.Powers;
using SlimeBoss.SlimeBossCode.Slimes;
using SlimeBoss.SlimeBossCode.Vfx;

namespace SlimeBoss.SlimeBossCode.Core;

public static class SlimeBossCmd
{
    public static IEnumerable<SlimeModel> GetSlimes(Player player)
    {
        return player.SlimeCreatures.Select(e => e.Monster).OfType<SlimeModel>();
    }

    /// <summary>Potency granted instead of duplicating an already-summoned slime type.</summary>
    private const int DuplicateSplitPotency = 2;


    /// <summary>
    /// "Consume - if the enemy has Weak, remove a stack of Weak and perform an additional effect." Always
    /// attempted explicitly by the card itself (like Schlurp) rather than gated behind an attack landing -
    /// there is no separate Goop resource anymore, Consume keys directly off the target's Weak stacks.
    /// </summary>
    public static async Task<bool> Consume(PlayerChoiceContext ctx, CardModel card, CardPlay? cardPlay,
        Func<Creature, Task> effect)
    {
        if (cardPlay == null) return false;
        // card.GetTargets() only resolves AoE target types; single-target cards (AnyEnemy etc.) need the
        // actually-clicked target from CardPlay - MyGetTargets handles both uniformly (see the
        // DuplicatedFormDoublesAoeCardsTargetingEnemies regression test for the AoE-side version of this).
        var targets = card.MyGetTargets(cardPlay.Target);
        var consumed = 0;
        foreach (var target in targets)
        {
            var weak = target.GetPower<WeakPower>();
            if (weak is not { Amount: > 0 }) continue;
            await PowerCmd.ModifyAmount(ctx, weak, -1, card.Owner.Creature, card);
            consumed++;
            await effect(target);
            if (target.CombatState != null)
                await SlimeBossHook.AfterConsumeEffect(target.CombatState, ctx, target, card.Owner.Creature);
        }

        return consumed > 0;
    }


    private static async Task RunCommand(PlayerChoiceContext ctx, Player player, SlimeModel slime, CardModel? source,
        Creature? forcedTarget, bool isAutomatic)
    {
        await slime.Command(ctx, forcedTarget);
        if (player.Creature.CombatState == null) return;
        await SlimeBossHook.AfterCommand(player.Creature.CombatState, ctx, player, slime, source, isAutomatic);
    }

    private static Task RunCommands(PlayerChoiceContext ctx, Player player, IEnumerable<SlimeModel> slimes,
        CardModel? cardSource, Creature? forcedTarget, bool isAutomatic)
    {
        return slimes.ToList().ForEachAsync(s => RunCommand(ctx, player, s, cardSource, forcedTarget, isAutomatic));
    }

    /// <summary>
    /// Commands a specific slime type (e.g. "Command Bruiser Slime"). If the player has not split into that
    /// slime yet, they Split into it first before it acts.
    /// </summary>
    public static async Task Command<T>(PlayerChoiceContext ctx, Player player, int amount,
        CardModel? cardSource = null, Creature? forcedTarget = null) where T : SlimeModel
    {
        for (var i = 0; i < amount; i++)
        {
            var slime = GetSlimes(player).OfType<T>().FirstOrDefault();
            if (slime == null)
            {
                await Split<T>(ctx, player);
                slime = GetSlimes(player).OfType<T>().FirstOrDefault();
            }

            if (slime != null)
                await RunCommands(ctx, player, [slime], cardSource, forcedTarget, isAutomatic: false);
        }
    }

    public static Task Command<T>(PlayerChoiceContext ctx, CardModel card)
        where T : SlimeModel
    {
        return Command<T>(ctx, card.Owner, card.DynamicVars["Command"].IntValue, card);
    }

    public static Task CommandAll(PlayerChoiceContext ctx, Player player, CardModel? cardSource = null,
        Creature? forcedTarget = null)
    {
        return RunCommands(ctx, player, GetSlimes(player).Reverse(), cardSource, forcedTarget, isAutomatic: false);
    }
    public static Task AutomaticCommandAll(PlayerChoiceContext ctx, Player player)
    {
        return RunCommands(ctx, player, GetSlimes(player).Reverse(), cardSource: null, forcedTarget: null,
            isAutomatic: true);
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
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static async Task<Creature?> Split(PlayerChoiceContext ctx, Player player, SlimeModel slimeModel)
    {
        var existing = player.GetSlime(slimeModel);
        if (existing == null)
        {
            return await SpawnSlime(ctx, player, slimeModel);
        }
        if (existing.Monster is not SlimeModel slime) return existing;
        switch (slime.SlimeType)
        {
            case SlimeType.Single:
                await PowerCmd.Apply<PotencyPower>(ctx, existing, DuplicateSplitPotency, player.Creature, null);
                break;
            case SlimeType.Counter:
                slime.SlimeAmount++;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return existing;
    }

 
    private static async Task<Creature?> SpawnSlime(PlayerChoiceContext ctx, Player player, SlimeModel slimeModel)
    {
        var slime = await AddSlime(player, slimeModel);
        if (player.Creature.CombatState == null) return slime;
        await SlimeBossHook.AfterSplit(player.Creature.CombatState, ctx, player, slimeModel);
        return slime;
    }

    private static async Task<Creature?> AddSlime(Player player, SlimeModel slimeModel)
    {
        var pet = player.Creature.CombatState?.CreateCreature(slimeModel.ToMutable(), player.Creature.Side, null);
        if (pet == null) return null;
        await PlayerCmd.AddPet(pet, player);
        Callable.From(() => PlaceNewSlime(player, pet)).CallDeferred();
        return pet;
    }


    /// <summary>Slimes fill a column of this height top to bottom before starting the next column.</summary>
    private const int GridHeight = 3;

    private static Vector2 GridOrigin => new(250f, -100f);
    private static Vector2 GridCellSize => new(150f, 100f);

    /// <summary>Grid slot each living slime occupies. A slot stays fixed until its slime leaves.</summary>
    private static readonly SpireField<Creature, int> SlimeSlots = new(() => UnassignedSlot);

    private const int UnassignedSlot = -1;

    private static int AssignSlot(Creature slime, List<Creature> allSlimes)
    {
        var existing = SlimeSlots.Get(slime);
        if (existing != UnassignedSlot) return existing;

        var taken = allSlimes.Select(e => SlimeSlots.Get(e)).ToHashSet();
        var slot = 0;
        while (taken.Contains(slot)) slot++;
        SlimeSlots.Set(slime, slot);
        return slot;
    }

    private static Vector2 SlotOffset(int slot)
    {
        var column = slot / GridHeight;
        var row = slot % GridHeight;
        return GridOrigin + new Vector2(column * GridCellSize.X, row * GridCellSize.Y);
    }

    /// <summary>Places a newly split slime in the lowest free grid slot. Existing slimes are never moved.</summary>
    private static void PlaceNewSlime(Player player, Creature pet)
    {
        var playerNode = NCombatRoom.Instance?.GetCreatureNode(player.Creature);
        var slimeNode = NCombatRoom.Instance?.GetCreatureNode(pet);
        if (playerNode == null || slimeNode == null) return;

        var slimes = player.Creature.Pets.Where(e => e.Monster is SlimeModel).ToList();
        var relativeOffset = SlotOffset(AssignSlot(pet, slimes));
        if (player.Creature.Side == CombatSide.Enemy) relativeOffset.X = -relativeOffset.X;

        slimeNode.ToggleIsInteractable(true);
        HideHealthBar(slimeNode);
        slimeNode.GlobalPosition = playerNode.GlobalPosition + relativeOffset;
        slimeNode.UpdateBounds(slimeNode.Visuals);
        if (pet.Monster is SlimeModel slime) slimeNode.AddChild(NSlimeCounter.Create(slimeNode, slime));
    }

    /// <summary>Height of the hidden HP bar (the power container's offset in creature_state_display.tscn).</summary>
    private static float HiddenHealthBarHeight => 20f;

    /// <summary>
    /// Slimes show no HP bar, so the powers and nameplate that sit below it would leave a gap under the slime.
    /// Lift both by the bar's height. The state display itself can't be moved: <c>AnimateIn</c> tweens its
    /// position back to the one it captured in <c>_Ready</c>. Must run before <c>UpdateBounds</c>, which is
    /// when the power container captures its own original position.
    /// </summary>
    private static void HideHealthBar(NCreature slimeNode)
    {
        var display = slimeNode._stateDisplay;
        display._healthBar.Visible = false;
        foreach (var name in new[] { "%PowerContainer", "%NameplateContainer" })
            display.GetNode<Control>(name).Position += Vector2.Up * HiddenHealthBarHeight;
    }
}
