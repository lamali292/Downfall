using BaseLib.Abstracts;
using BaseLib.Utils;
using Downfall.Code.Cards.Guardian.Abstract;
using Downfall.Code.Core.Champ;
using Downfall.Code.Events;
using Downfall.Code.RestSiteOptions;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Downfall.Code.Core.Guardian;

public class GuardianModel() : CustomSingletonModel(true, true)
{
    private static readonly SpireField<Player, GuardianModeModel> ActiveStance =
        new(DownfallModelDb.GuardianMode<GuardianNormalMode>);

    public static GuardianModeModel GetModeModel(Player player)
    {
        return ActiveStance[player] ?? DownfallModelDb.GuardianMode<GuardianNormalMode>();
    }

    public static bool IsInMode<T>(Player player) where T : GuardianModeModel
    {
        return ActiveStance[player] is T;
    }
    
    public static async Task SetMode<T>(PlayerChoiceContext ctx, Player player) where T : GuardianModeModel
    {
        await SetMode(ctx, player, DownfallModelDb.GuardianMode<T>());
    }
    
    private static async Task SetMode(PlayerChoiceContext ctx, Player player, GuardianModeModel newCanonical)
    {
        var current = ActiveStance[player];
        if (current?.GetType() == newCanonical.GetType()) return;

        if (current != null)
            await current.OnExit(ctx);

        var mutable = newCanonical.ToMutable(player);
        ActiveStance[player] = mutable;
        await mutable.OnEnter(ctx);

        TriggerStanceAnimation(player);
        await DownfallHook.OnGuardianModeChange(player.Creature.CombatState!, ctx, player, current!, ActiveStance[player]!);
    }
    
    
    private static void TriggerStanceAnimation(Player player)
    {
        Callable.From(() =>
        {
            var creatureNode = NCombatRoom.Instance?.GetCreatureNode(player.Creature);
            var animState = creatureNode?.SpineAnimation.GetAnimationState();
            if (animState == null) return;
            animState.GetCurrent(0).SetMixDuration(0.3f);
            creatureNode?.SetAnimationTrigger("Idle");
            animState.GetCurrent(0).SetMixDuration(0.3f);
        }).CallDeferred();
    }

    public override bool TryModifyRestSiteOptions(Player player, ICollection<RestSiteOption> options)
    {
        if (player.Character is not Character.Guardian) return false;
        var gems = PileType.Deck.GetPile(player).Cards.Where(e => e is IGemCard).ToList();
        if (gems.Count == 0) return false;
        options.Add(new GemRestSiteOption(player));
        return true;
    }
}