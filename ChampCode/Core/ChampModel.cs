// ChampModel.cs

using BaseLib.Abstracts;
using Champ.ChampCode.Events;
using Champ.ChampCode.Extensions;
using Champ.ChampCode.Stance;
using Champ.ChampCode.Vfx;
using Downfall.DownfallCode.Core;
using Godot;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Champ.ChampCode.Core;

public class ChampModel() : CustomSingletonModel(HookType.Combat)
{
    private static readonly PlayerField<ChampStanceModel> ActiveStance =
        new(_ => null);

    public static ChampStanceModel GetStanceModel(Player player)
    {
        var stance = ActiveStance[player];
        if (stance != null) return stance;
        stance = ChampModelDb.ChampStance<ChampNoStance>().ToMutable(player);
        SetStanceInternal(player, stance);
        return stance;
    }
    
    private static void SetStanceInternal(Player player, ChampStanceModel model)
    {
        model.AssertMutable();
        ActiveStance[player] = model;
    }

    
    
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
    

    public static bool IsInStance<T>(Player player) where T : ChampStanceModel
    {
        return GetStanceModel(player) is T;
    }


    public static async Task SetStance<T>(PlayerChoiceContext ctx, Player player) where T : ChampStanceModel
    {
        await SetStance(ctx, player, ChampModelDb.ChampStance<T>());
    }

    private static async Task SetStance(PlayerChoiceContext ctx, Player player, ChampStanceModel newCanonical)
    {
        newCanonical.AssertCanonical();
        var oldStance = GetStanceModel(player);
        oldStance.AssertMutable();
        if (oldStance.GetType() == newCanonical.GetType() || oldStance == newCanonical) return;

        await oldStance.OnExit(ctx);

        var newStance = newCanonical.ToMutable(player);
        SetStanceInternal(player, newStance);
        await newStance.OnEnter(ctx);

        TriggerStanceAnimation(player);
        await ChampHook.OnChampStanceChange(player.Creature.CombatState!, ctx, player, oldStance, newStance);
        RefreshStanceDisplay(player, newCanonical);
    }


    public override Task BeforeCombatStart()
    {
        return Task.CompletedTask;
    }


    private static void TriggerStanceAnimation(Player player)
    {
        Callable.From(() =>
        {
            var creatureNode = NCombatRoom.Instance?.GetCreatureNode(player.Creature);
            if (creatureNode?.Visuals is not NChampCreatureVisuals champVisuals) return;

            champVisuals.CurrentStance = GetStanceModel(player) switch
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
        if (!LocalContext.IsMe(player)) return;
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