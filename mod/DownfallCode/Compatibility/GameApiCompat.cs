using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Runs;

namespace Downfall.DownfallCode.Compatibility;

/// <summary>Small signature changes between game versions that need no more than a reflection call.</summary>
public static class GameApiCompat
{
    private static readonly MethodInfo? ToggleIsInteractableMethod =
        AccessTools.Method(typeof(NCreature), "ToggleIsInteractable", [typeof(bool)]);

    private static readonly MethodInfo? SetIsInteractableMethod =
        AccessTools.Method(typeof(NCreature), "SetIsInteractable", [typeof(bool), typeof(bool)]);

    private static readonly ConstructorInfo CombatStateCtor =
        typeof(CombatState).GetConstructors().OrderByDescending(c => c.GetParameters().Length).First();

    /// <summary>
    ///     `NCreature.ToggleIsInteractable(on)` on older builds, `SetIsInteractable(on, changeVisibilityImmediately)`
    ///     on newer ones.
    /// </summary>
    public static void ToggleIsInteractableCompat(this NCreature node, bool on)
    {
        if (ToggleIsInteractableMethod != null)
            ToggleIsInteractableMethod.Invoke(node, [on]);
        else
            SetIsInteractableMethod?.Invoke(node, [on, false]);
    }

    private static readonly MethodInfo PlayOnCreatureCenterMethod =
        AccessTools.Method(typeof(VfxCmd), "PlayOnCreatureCenter");

    /// <summary>`VfxCmd.PlayOnCreatureCenter(target, path)`; newer builds append an optional `forceIfDead`.</summary>
    public static void PlayOnCreatureCenterCompat(Creature target, string path) =>
        PlayOnCreatureCenterMethod.Invoke(null, FillDefaults(PlayOnCreatureCenterMethod.GetParameters(), target, path));

    private static readonly ConstructorInfo CardRewardOfferCtor = typeof(CardReward).GetConstructors().First(c =>
        c.GetParameters() is { Length: >= 4 } ps && ps[0].ParameterType == typeof(IEnumerable<CardModel>));

    /// <summary>
    ///     `new CardReward(cardsToOffer, source, player, rerollOptions)`; newer builds append optional parameters
    ///     (`selectorStack`) that a compiled call would otherwise bake into the signature.
    /// </summary>
    public static CardReward CreateCardReward(IEnumerable<CardModel> cards, CardCreationSource source, Player player,
        CardCreationOptions rerollOptions) =>
        (CardReward)CardRewardOfferCtor.Invoke(
            FillDefaults(CardRewardOfferCtor.GetParameters(), cards, source, player, rerollOptions));

    /// <summary>Arguments for a call: <paramref name="given"/> first, every remaining parameter at its default.</summary>
    private static object?[] FillDefaults(ParameterInfo[] parameters, params object?[] given) =>
        parameters.Select((p, i) => i < given.Length ? given[i] : p.DefaultValue).ToArray();

    private static readonly MethodInfo? DebugRandomizeRngMethod = AccessTools.Method(typeof(EncounterModel), "DebugRandomizeRng");

    /// <summary>`EncounterModel.DebugRandomizeRng()` returns void on older builds and the seed on newer ones; the result is unused.</summary>
    public static void DebugRandomizeRngCompat(this EncounterModel encounter) =>
        DebugRandomizeRngMethod?.Invoke(encounter, null);

    private static readonly MethodInfo CreateForTestMethod = AccessTools.Method(typeof(RunState), "CreateForTest");

    /// <summary>
    ///     `RunState.CreateForTest(players, seed)`. Its optional parameters differ between builds (e.g. `isCustomGame`
    ///     was added), and a compiled call bakes in the defaults positionally, so the call is made by parameter name.
    /// </summary>
    public static RunState CreateRunStateForTest(IReadOnlyList<Player> players, string seed)
    {
        var args = CreateForTestMethod.GetParameters().Select(p => p.Name switch
        {
            "players" => players,
            "seed" => seed,
            _ => p.DefaultValue
        }).ToArray();
        return (RunState)CreateForTestMethod.Invoke(null, args)!;
    }

    /// <summary>
    ///     Builds a <see cref="CombatState"/> for a run. The trailing constructor parameters changed between builds
    ///     (<c>multiplayerScalingModel</c> became <c>runModels</c> + <c>dominion</c>), so each one is filled from the
    ///     same-named (PascalCase) property of <paramref name="run"/> when it exists.
    /// </summary>
    public static CombatState CreateCombatState(EncounterModel encounter, RunState run)
    {
        var args = CombatStateCtor.GetParameters().Select(p =>
        {
            if (p.Name == "encounter") return encounter;
            if (p.Name == "runState") return run;
            var prop = p.Name == null ? null : AccessTools.Property(typeof(RunState), char.ToUpperInvariant(p.Name[0]) + p.Name[1..]);
            return prop?.GetValue(run);
        }).ToArray();
        return (CombatState)CombatStateCtor.Invoke(args);
    }
}
