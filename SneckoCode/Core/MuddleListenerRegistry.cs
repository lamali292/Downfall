using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Snecko.SneckoCode.Core;

/// <summary>
///     One model's Muddle hooks, whichever mod's interfaces it implements.
/// </summary>
public sealed class MuddleListener(AbstractModel model)
{
    public AbstractModel Model { get; } = model;
    public Func<CardModel, int, int>? ModifyMaxCost { get; set; }
    public Func<PlayerChoiceContext, CardModel, Task>? AfterModifyMaxCost { get; set; }
    public Func<CardModel, bool>? ShouldPermanent { get; set; }
    public Func<PlayerChoiceContext, CardModel, Task>? AfterShouldPermanent { get; set; }
    public Func<PlayerChoiceContext, CardModel, Task>? AfterMuddled { get; set; }
}

/// <summary>
///     Collects every Muddle listener in a combat: this mod's own interfaces plus any other mod's copy of them.
///     A mod joins by calling <see cref="Register" /> with the namespace its three interfaces live in.
/// </summary>
public static class MuddleListenerRegistry
{
    private static readonly List<string> Namespaces = [];
    private static List<ForeignMuddleHooks>? _resolved;

    /// <summary>
    ///     Adds a mod's Muddle interface namespace (e.g. <c>"Snecko.SneckoCode.Core"</c>). Safe to call for a mod that
    ///     may not be installed, and more than once. Registering this mod's own namespace is ignored.
    /// </summary>
    public static void Register(string interfaceNamespace)
    {
        if (Namespaces.Contains(interfaceNamespace)) return;
        Namespaces.Add(interfaceNamespace);
        _resolved = null;
    }

    private static List<ForeignMuddleHooks> Resolved => _resolved ??= Namespaces
        .Select(ForeignMuddleHooks.Find)
        .OfType<ForeignMuddleHooks>()
        .ToList();

    public static List<MuddleListener> Collect(ICombatState cs)
    {
        List<MuddleListener> listeners = [];
        if (CombatManager.Instance.IsOverOrEnding && !CombatManager.Instance.IsStarting)
            return listeners;

        foreach (var model in cs.IterateHookListeners())
        {
            var listener = new MuddleListener(model);
            if (model is IMaxMuddleCost maxCost)
            {
                listener.ModifyMaxCost = maxCost.ModifyMaxMuddleCost;
                listener.AfterModifyMaxCost = maxCost.AfterModifyingMaxMuddleCost;
            }

            if (model is IShouldPermanentMuddleListener permanent)
            {
                listener.ShouldPermanent = permanent.ShouldPermanentMuddle;
                listener.AfterShouldPermanent = permanent.AfterShouldPermanentMuddle;
            }

            if (model is IAfterCardMuddled afterMuddled)
                listener.AfterMuddled = afterMuddled.AfterCardMuddled;

            foreach (var foreign in Resolved)
                foreign.Bind(listener);

            if (listener.ModifyMaxCost != null || listener.ShouldPermanent != null || listener.AfterMuddled != null)
                listeners.Add(listener);
        }

        return listeners;
    }

    /// <summary>One foreign mod's three Muddle interfaces and their reflected methods.</summary>
    private sealed record ForeignMuddleHooks(
        Type MaxCostType,
        MethodInfo ModifyMaxCost,
        MethodInfo AfterModifyMaxCost,
        Type AfterMuddledType,
        MethodInfo AfterMuddled,
        Type PermanentType,
        MethodInfo ShouldPermanent,
        MethodInfo AfterShouldPermanent)
    {
        /// <summary>Fills in the listener's hooks for each foreign interface its model implements.</summary>
        public void Bind(MuddleListener listener)
        {
            var model = listener.Model;
            if (MaxCostType.IsInstanceOfType(model))
            {
                listener.ModifyMaxCost = (card, value) => (int)ModifyMaxCost.Invoke(model, [card, value])!;
                listener.AfterModifyMaxCost = (ctx, card) => (Task)AfterModifyMaxCost.Invoke(model, [ctx, card])!;
            }

            if (PermanentType.IsInstanceOfType(model))
            {
                listener.ShouldPermanent = card => (bool)ShouldPermanent.Invoke(model, [card])!;
                listener.AfterShouldPermanent = (ctx, card) =>
                    (Task)AfterShouldPermanent.Invoke(model, [ctx, card])!;
            }

            if (AfterMuddledType.IsInstanceOfType(model))
                listener.AfterMuddled = (ctx, card) => (Task)AfterMuddled.Invoke(model, [ctx, card])!;
        }

        public static ForeignMuddleHooks? Find(string ns)
        {
            var maxCost = AccessTools.TypeByName($"{ns}.IMaxMuddleCost");
            var afterMuddled = AccessTools.TypeByName($"{ns}.IAfterCardMuddled");
            var permanent = AccessTools.TypeByName($"{ns}.IShouldPermanentMuddleListener");
            if (maxCost == null || afterMuddled == null || permanent == null) return null;
            
            if (maxCost.Assembly == typeof(MuddleListenerRegistry).Assembly) return null;

            var modify = maxCost.GetMethod("ModifyMaxMuddleCost");
            var afterModify = maxCost.GetMethod("AfterModifyingMaxMuddleCost");
            var after = afterMuddled.GetMethod("AfterCardMuddled");
            var should = permanent.GetMethod("ShouldPermanentMuddle");
            var afterShould = permanent.GetMethod("AfterShouldPermanentMuddle");
            if (modify == null || afterModify == null || after == null || should == null || afterShould == null)
                return null;

            return new ForeignMuddleHooks(maxCost, modify, afterModify, afterMuddled, after, permanent, should,
                afterShould);
        }
    }
}
