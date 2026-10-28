using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Collector.CollectorCode.Patches;

/// <summary>
///     A downed (0 HP) Torchhead still attacks when commanded, at half damage. The game refuses that in two
///     places: <c>AttackCommand.Execute</c> returns when its attacker <c>IsDead</c>, and
///     <c>CreatureCmd.Damage</c> returns an empty result when its dealer <c>IsDead</c>. This swaps just those two
///     checks (attacker / dealer, not the targets) for <see cref="IsDeadAsAttacker" />, which treats a dead
///     Torchhead as able to act. Both are async methods, so the patch goes on each state machine's
///     <c>MoveNext</c>.
/// </summary>
[HarmonyPatch]
internal static class DownedTorchheadAttackPatch
{
    private static readonly MethodInfo IsDeadGetter = AccessTools.PropertyGetter(typeof(Creature), nameof(Creature.IsDead));

    private static readonly MethodInfo Replacement =
        AccessTools.Method(typeof(DownedTorchheadAttackPatch), nameof(IsDeadAsAttacker));

    public static bool IsDeadAsAttacker(Creature creature) => creature.IsDead && creature.Monster is not TorchheadMonsterModel;

    private static IEnumerable<MethodBase> TargetMethods()
    {
        yield return StateMachineMoveNext(typeof(AttackCommand), "Execute", requiredField: null);
        foreach (var damage in typeof(CreatureCmd).GetNestedTypes(AccessTools.all)
                     .Where(t => t.Name.StartsWith("<Damage>d__") && AccessTools.Field(t, "dealer") != null))
            yield return AccessTools.Method(damage, "MoveNext");
    }

    private static MethodBase StateMachineMoveNext(Type owner, string method, string? requiredField)
    {
        var machine = owner.GetNestedTypes(AccessTools.all)
                          .FirstOrDefault(t => t.Name.StartsWith($"<{method}>d__"))
                      ?? throw new MissingMemberException($"{owner.Name}.{method} state machine not found");
        return AccessTools.Method(machine, "MoveNext");
    }

    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var code = instructions.ToList();
        for (var i = 1; i < code.Count; i++)
        {
            if (!code[i].Calls(IsDeadGetter) || !LoadsAttacker(code[i - 1])) continue;
            code[i] = new CodeInstruction(OpCodes.Call, Replacement) { labels = code[i].labels };
        }

        return code;
    }

    /// <summary>The instruction right before <c>.IsDead</c> loads the attacker (AttackCommand) or the dealer (Damage).</summary>
    private static bool LoadsAttacker(CodeInstruction instruction) =>
        instruction.operand is MemberInfo { Name: "get_Attacker" or "dealer" };
}
