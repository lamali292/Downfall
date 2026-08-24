// ChampModel.cs

using System.Runtime.CompilerServices;
using BaseLib.Abstracts;
using BaseLib.Utils;
using Champ.ChampCode.Events;
using Champ.ChampCode.Extensions;
using Champ.ChampCode.Stance;
using Champ.ChampCode.Vfx;
using Downfall.DownfallCode.Core;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Champ.ChampCode.Core;

public class ChampModel() : CustomSingletonModel(HookType.Combat)
{
    private static readonly PlayerField<ChampStanceModel> ActiveStance =
        new(ChampModelDb.ChampStance<ChampNoStance>);

    private static readonly PlayerField<NChampStanceDisplay> StanceDisplays = new(() => null);

    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        var card = cardPlay.Card;
        var owner = card.Owner;
        var stance = owner.ChampStance;
        var ignoreChargeCap = ChampHook.IgnoreChargeCap(owner.Creature.CombatState!, owner);
        if (card.Type == CardType.Skill && (ignoreChargeCap || stance.Charges > 0))
        {
            if (!ignoreChargeCap)
            {
                stance.Charges--;
                RefreshDisplay(owner);
            }

            await stance.SkillBonus(new BlockingPlayerChoiceContext());
        }
    }

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
    
   
    public static async Task SetStance<T>(PlayerChoiceContext ctx, Player player) where T : ChampStanceModel
    {
        await SetStance(ctx, player, ChampModelDb.ChampStance<T>());
    }

    private static async Task SetStance(PlayerChoiceContext ctx, Player player, ChampStanceModel newCanonical)
    {
        var current = ActiveStance[player];
        if (current?.GetType() == newCanonical.GetType() || current == newCanonical) return;

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


    public override Task BeforeCombatStart()
    {
        var state = CombatManager.Instance.DebugOnlyGetState();
        if (state == null) return Task.CompletedTask;
        foreach (var player in state.Players)
            ActiveStance[player] = ChampModelDb.ChampStance<ChampNoStance>();
        return Task.CompletedTask;
    }


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
                _ => NChampCreatureVisuals.Stance.Normal
            };

            champVisuals.OnAnimationTrigger("Idle");
        }).CallDeferred();
    }

    public static void RefreshDisplay(Player player)
    {
        StanceDisplays.Get(player)?.Refresh();
    }


    
    private static void RefreshStanceDisplay(Player player, ChampStanceModel newCanonical)
    {
        if (!LocalContext.IsMe(player)) return;;
        Callable.From(() =>
        {
            var existing = StanceDisplays.Get(player);

            // If the current display is running its exit tween, treat it as dead
            if (existing != null && (!GodotObject.IsInstanceValid(existing) || existing.IsExiting))
            {
                StanceDisplays.Set(player, null);
                existing = null;
            }

            if (newCanonical is ChampNoStance)
            {
                if (existing == null) return;
                existing.AnimOutAndFree();
                StanceDisplays.Set(player, null);
                return;
            }

            if (existing == null)
            {
                var display = NChampStanceDisplay.Show(player);
                if (display != null) 
                    StanceDisplays.Set(player, display);
            }
            else
            {
                existing.Refresh();
            }
        }).CallDeferred();
    }
}