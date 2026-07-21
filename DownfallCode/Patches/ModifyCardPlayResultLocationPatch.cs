using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using BaseLib.Utils;
using Downfall.DownfallCode.Compatibility;
using Downfall.DownfallCode.Events;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Patches;

/// <summary>
///     New game version only. Compiled against the OLD assembly, so CardLocation must never
///     appear in typed code — accessed via reflection/Traverse only.
///     Must only be added when CardLocation exists at runtime (see DownfallPatchManager).
/// </summary>
[HarmonyPatch]
public static class ModifyCardPlayResultLocationNewPatch
{
    private static readonly Type? CardLocationType =
        AccessTools.TypeByName("MegaCrit.Sts2.Core.Entities.Cards.CardLocation");

    static MethodBase TargetMethod() =>
        AccessTools.Method(typeof(Hook), "ModifyCardPlayResultLocation");

    // __result as object: Harmony boxes the CardLocation struct for us
    static void Postfix(
        ICombatState combatState,
        CardModel card,
        bool isAutoPlay,
        ResourceInfo resources,
        ref object __result,
        ref IEnumerable<AbstractModel> modifiers)
    {
        var tr = Traverse.Create(__result);
        var player   = tr.Field("player").GetValue<Player>();
        var pileType = tr.Field("pileType").GetValue<PileType>();
        var position = tr.Field("position").GetValue<CardPilePosition>();

        var result = HookUtils.Modify<IModifyCardPlayResultLocation, CardLocationCompatiblity>(
            combatState,
            new CardLocationCompatiblity(player, pileType, position),
            (m, loc) => m.ModifyCardPlayResultLocationCompability(card, isAutoPlay, resources, loc),
            out var compatModifiers);

        __result = Activator.CreateInstance(CardLocationType!,
            result.Player, result.PileType, result.Position)!;

        var added = compatModifiers.OfType<AbstractModel>().ToList();
        if (added.Count > 0)
            modifiers = modifiers.Concat(added).ToList();
    }
}


/// <summary>
///     Old game version only: dispatches <see cref="IModifyCardPlayResultLocation" /> compat listeners
///     after the vanilla <c>Hook.ModifyCardPlayResultPileTypeAndPosition</c> loop.
///     The old engine has no Player in card locations, so the compat struct carries
///     <c>Player = null</c> and any player redirection returned by listeners is dropped.
///     Must only be added when <c>CardLocation</c> does NOT exist (see DownfallPatchManager).
/// </summary>
[HarmonyPatch]
public static class ModifyCardPlayResultLocationOldPatch
{
    static MethodBase TargetMethod() =>
        AccessTools.Method(typeof(Hook), "ModifyCardPlayResultPileTypeAndPosition");

    static void Postfix(
        ICombatState combatState,
        CardModel card,
        bool isAutoPlay,
        ResourceInfo resources,
        ref (PileType, CardPilePosition) __result,
        ref IEnumerable<AbstractModel> modifiers)
    {
        var result = HookUtils.Modify<IModifyCardPlayResultLocation, CardLocationCompatiblity>(
            combatState,
            new CardLocationCompatiblity(card.Owner, __result.Item1, __result.Item2),
            (m, loc) => m.ModifyCardPlayResultLocationCompability(card, isAutoPlay, resources, loc),
            out var compatModifiers);

        __result = (result.PileType, result.Position);

        var added = compatModifiers.OfType<AbstractModel>().ToList();
        if (added.Count > 0)
            modifiers = modifiers.Concat(added).ToList();
    }
}



internal static class OnPlayWrapperStateMachine
{
    public static MethodBase MoveNext()
    {
        var onPlay = AccessTools.Method(typeof(CardModel), "OnPlayWrapper")
                     ?? throw new MissingMethodException("CardModel.OnPlayWrapper not found");
        var sm = onPlay.GetCustomAttribute<AsyncStateMachineAttribute>()
                 ?? throw new InvalidOperationException("OnPlayWrapper has no async state machine");
        return AccessTools.Method(sm.StateMachineType, "MoveNext")
               ?? throw new MissingMethodException("MoveNext not found");
    }
}

// ==================== NEW VERSION (add only when CardLocation exists) ====================

[HarmonyPatch]
internal static class AfterModifyingLocationNewPatch
{
    private static readonly Type CardLocationType =
        AccessTools.TypeByName("MegaCrit.Sts2.Core.Entities.Cards.CardLocation")!;

    private static readonly MethodInfo Vanilla =
        AccessTools.Method(typeof(AbstractModel), "AfterModifyingCardPlayResultLocation");

    static MethodBase TargetMethod() => OnPlayWrapperStateMachine.MoveNext();

    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var code = new List<CodeInstruction>(instructions);
        var bridge = AccessTools.Method(typeof(AfterModifyingLocationNewPatch), nameof(Bridge));
        var found = false;

        for (var i = 0; i < code.Count; i++)
        {
            if (!code[i].Calls(Vanilla)) continue;

            // Stack: AbstractModel, CardModel, CardLocation(struct).
            // Box the struct, then call the bridge instead of the virtual.
            var box = new CodeInstruction(OpCodes.Box, CardLocationType);
            box.labels.AddRange(code[i].labels);
            box.blocks.AddRange(code[i].blocks);
            code[i] = new CodeInstruction(OpCodes.Call, bridge);
            code.Insert(i, box);
            i++;
            found = true;
        }

        if (!found)
            throw new InvalidOperationException(
                "AfterModifyingCardPlayResultLocation call site not found in OnPlayWrapper");

        return code;
    }

    public static Task Bridge(AbstractModel model, CardModel card, object boxedLocation)
    {
        // Invoke uses virtual dispatch — vanilla overrides still run.
        var vanilla = (Task)Vanilla.Invoke(model, new[] { card, boxedLocation })!;
        if (model is not IModifyCardPlayResultLocation compat) return vanilla;

        var tr = Traverse.Create(boxedLocation);
        var loc = new CardLocationCompatiblity(
            tr.Field("player").GetValue<Player>(),
            tr.Field("pileType").GetValue<PileType>(),
            tr.Field("position").GetValue<CardPilePosition>());

        return Chain(vanilla, compat, card, loc);

        static async Task Chain(Task orig, IModifyCardPlayResultLocation c, CardModel cd,
            CardLocationCompatiblity l)
        {
            await orig;
            await c.AfterModifyingCardPlayResultLocationCompability(cd, l);
        }
    }
}

// ==================== OLD VERSION (add only when CardLocation does not exist) ====================

[HarmonyPatch]
internal static class AfterModifyingLocationOldPatch
{
    private static readonly MethodInfo Vanilla =
        AccessTools.Method(typeof(AbstractModel), "AfterModifyingCardPlayResultPileOrPosition");

    static MethodBase TargetMethod() => OnPlayWrapperStateMachine.MoveNext();

    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var code = new List<CodeInstruction>(instructions);
        var bridge = AccessTools.Method(typeof(AfterModifyingLocationOldPatch), nameof(Bridge));
        var found = false;

        for (var i = 0; i < code.Count; i++)
        {
            if (!code[i].Calls(Vanilla)) continue;

            // Stack: AbstractModel, CardModel, PileType, CardPilePosition — matches Bridge 1:1.
            code[i] = new CodeInstruction(OpCodes.Call, bridge)
            {
                labels = code[i].labels,
                blocks = code[i].blocks
            };
            found = true;
        }

        if (!found)
            throw new InvalidOperationException(
                "AfterModifyingCardPlayResultPileOrPosition call site not found in OnPlayWrapper");

        return code;
    }

    public static Task Bridge(AbstractModel model, CardModel card, PileType pileType, CardPilePosition position)
    {
        var vanilla = (Task)Vanilla.Invoke(model, [card, pileType, position])!;
        if (model is not IModifyCardPlayResultLocation compat) return vanilla;

        return Chain(vanilla, compat, card,
            new CardLocationCompatiblity(card.Owner, pileType, position));

        static async Task Chain(Task orig, IModifyCardPlayResultLocation c, CardModel cd,
            CardLocationCompatiblity l)
        {
            await orig;
            await c.AfterModifyingCardPlayResultLocationCompability(cd, l);
        }
    }
}