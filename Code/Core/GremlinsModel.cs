using Downfall.Code.Character;
using Downfall.Code.Commands;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.Code.Core;

public class GremlinsModel : AbstractModel
{
    public override bool ShouldReceiveCombatHooks => true;

    private static void LogState(string context, GremlinsRunModel.GremlinState state)
    {
        GD.Print($"[Gremlins:{context}] activeIndex={state.ActiveIndex} count={state.Gremlins.Count}");
        for (var i = 0; i < state.Gremlins.Count; i++)
        {
            var g = state.Gremlins[i];
            GD.Print($"  [{i}] {g.Name} hp={g.CurrentHp}/{g.MaxHp} alive={g.IsAlive} petOwner={g.PetOwner?.Character?.GetType().Name ?? "null"}");
        }
    }

    public override Creature ModifyUnblockedDamageTarget(
        Creature originalTarget, decimal amount, ValueProp props, Creature? dealer)
    {
        if (originalTarget.Player?.Character is not Gremlins)
            return originalTarget;

        var state = GremlinsRunModel.GetState(originalTarget.Player);
        var active = state.Active;

        GD.Print($"[Gremlins:ModifyUnblockedDamageTarget] originalTarget={originalTarget.Name} amount={amount} dealer={dealer?.Name ?? "null"}");
        LogState("ModifyUnblockedDamageTarget", state);
        GD.Print($"  -> active={active?.Name ?? "null"} isAlive={active?.IsAlive}");

        return active is { IsAlive: true } ? active : originalTarget;
    }

    public override decimal ModifyHpLostAfterOsty(
        Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target.Player?.Character is not Gremlins)
            return amount;

        GD.Print($"[Gremlins:ModifyHpLostAfterOsty] target={target.Name} amount={amount} -> blocking overkill, returning 0");
        return 0m;
    }

    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext, Creature target, DamageResult result,
        ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        var gremlin = result.Receiver;

        GD.Print($"[Gremlins:AfterDamageReceived] target={target.Name} result.Receiver={gremlin.Name}");
        GD.Print($"  result.UnblockedDamage={result.UnblockedDamage} WasTargetKilled={result.WasTargetKilled}");
        GD.Print($"  gremlin.PetOwner={gremlin.PetOwner?.Character?.GetType().Name ?? "null"}");

        if (gremlin.PetOwner?.Character is not Gremlins)
        {
            GD.Print("  -> not a Gremlins pet, skipping");
            return;
        }

        if (!result.WasTargetKilled)
        {
            GD.Print("  -> not killed, skipping");
            return;
        }

        var player = gremlin.PetOwner;
        var state = GremlinsRunModel.GetState(player);
        LogState("AfterDamageReceived", state);

        var gremlinIndex = state.Gremlins.IndexOf(gremlin);
        GD.Print($"  gremlinIndex={gremlinIndex}");

        if (gremlinIndex < 0)
        {
            GD.Print("  -> gremlin not found in state list, skipping");
            return;
        }

        GD.Print($"  -> calling KillGremlin({gremlinIndex})");
        GremlinsCmd.KillGremlin(player.Creature, gremlinIndex);

        var next = state.GetNextLivingIndex();
        GD.Print($"  -> next living index={next}");

        if (next < 0)
        {
            GD.Print("  -> no living gremlins, killing player");
            await CreatureCmd.Kill(player.Creature);
            return;
        }

        state.ActiveIndex = next;
        GD.Print($"  -> switching to gremlin {next} ({state.Gremlins[next].Name})");
        GremlinsCmd.SwitchGremlin(player.Creature, next);
    }

    public override Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Character is not Gremlins) return Task.CompletedTask;

        var player = cardPlay.Card.Owner;
        var state = GremlinsRunModel.GetState(player);

        GD.Print($"[Gremlins:AfterCardPlayed] card={cardPlay.Card.GetType().Name}");
        LogState("AfterCardPlayed", state);

        var next = state.GetNextLivingIndex();
        GD.Print($"  -> next living index={next}");

        if (next < 0)
        {
            GD.Print("  -> no next living gremlin, skipping");
            return Task.CompletedTask;
        }

        state.ActiveIndex = next;
        GD.Print($"  -> switching to gremlin {next} ({state.Gremlins[next].Name})");
        GremlinsCmd.SwitchGremlin(player.Creature, next);
        return Task.CompletedTask;
    }

    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    {
        GD.Print($"[Gremlins:AfterDeath] creature={creature.Name} deathAnimLength={deathAnimLength} petOwner={creature.PetOwner?.Character?.GetType().Name ?? "null"}");

        if (creature.PetOwner?.Character is not Gremlins) return;

        var player = creature.PetOwner;
        var state = GremlinsRunModel.GetState(player);
        LogState("AfterDeath", state);

        var gremlinIndex = state.Gremlins.IndexOf(creature);
        GD.Print($"  gremlinIndex={gremlinIndex}");
        if (gremlinIndex < 0) return;

        // Wait for death animation to finish before switching
        if (deathAnimLength > 0)
        {
            GD.Print($"  -> waiting {deathAnimLength}s for death anim");
            await Cmd.Wait(deathAnimLength);
        }

        // GremlinsCmd.KillGremlin(player.Creature, gremlinIndex);

        // Small pause so the kill anim registers visually
        

        var next = state.GetNextLivingIndex();
        GD.Print($"  next={next}");

        if (next < 0)
        {
            GD.Print("  -> no living gremlins, killing player");
            await CreatureCmd.Kill(player.Creature, true);
            return;
        }
        await Cmd.CustomScaledWait(0.4f, 0.6f);

        state.ActiveIndex = next;
        GD.Print($"  -> switching to {next} ({state.Gremlins[next].Name})");
        GremlinsCmd.SwitchGremlin(player.Creature, next);
    }
}

[HarmonyPatch(typeof(NCreature), nameof(NCreature.GetCurrentAnimationTimeRemaining))]
class GremlinDeathAnimTimePatch
{
    private static readonly HashSet<NCreature> _dyingGremlins = new();

    public static void MarkAsDying(NCreature creature) => _dyingGremlins.Add(creature);

    public static bool Prefix(NCreature __instance, ref float __result)
    {
        if (!_dyingGremlins.Contains(__instance)) return true;
        _dyingGremlins.Remove(__instance);
        __result = 0f;
        return false;
    }
}

[HarmonyPatch(typeof(NCreature), nameof(NCreature.StartDeathAnim))]
class GremlinDeathAnimStartPatch
{
    public static void Prefix(NCreature __instance)
    {
        if (__instance.Entity.Monster is GremlinsMonsterModel)
            GremlinDeathAnimTimePatch.MarkAsDying(__instance);
    }
}