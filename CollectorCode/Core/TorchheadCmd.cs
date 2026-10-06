using Collector.CollectorCode.Events;
using Collector.CollectorCode.Extensions;
using Collector.CollectorCode.Powers;
using Downfall.DownfallCode.Commands;
using Godot;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Collector.CollectorCode.Core;

public static class TorchheadCmd
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
        var torchhead = await PetSummonCmd.Summon<TorchheadMonsterModel, TorchheadPower>(ctx, summoner, hp, source);
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
}
