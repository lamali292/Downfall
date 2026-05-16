using HarmonyLib;
using MegaCrit.Sts2.Core.Models;

namespace BaseLib.Abstracts;

/// <summary>
/// Base class for custom run modifiers added by mods.
/// Automatically registered into <see cref="ModelDb.GoodModifiers"/> or <see cref="ModelDb.BadModifiers"/>
/// based on <see cref="Alignment"/>.
/// </summary>
public abstract class CustomModifierModel : ModifierModel, ICustomModel
{
    /// <summary>
    /// Whether this modifier appears in the good or bad modifiers list.
    /// </summary>
    public abstract ModifierAlignment Alignment { get; }
    
    /// <summary>
    /// Other modifiers that cannot be active at the same time as this one.
    /// Exclusivity is automatically symmetric — if A excludes B, ticking A unticks B and vice versa.
    /// Return empty if no exclusivity is needed.
    /// </summary>
    public virtual IEnumerable<ModifierModel> MutuallyExclusiveGroup => [];
    
    /// <summary>
    /// Position within the good or bad modifiers list relative to vanilla modifiers.
    /// Negative values insert before vanilla modifiers, positive values insert after.
    /// </summary>
    public virtual int SortOrder => 0;
}

/// <summary>
/// Determines whether a custom modifier appears in the good or bad modifiers list
/// on the custom run screen.
/// </summary>
public enum ModifierAlignment
{
    /// <summary>
    /// The modifier does not appear in either list.
    /// </summary>
    None,

    /// <summary>
    /// The modifier appears in <see cref="ModelDb.GoodModifiers"/>,
    /// shown as a beneficial run modifier on the custom run screen.
    /// </summary>
    Good,

    /// <summary>
    /// The modifier appears in <see cref="ModelDb.BadModifiers"/>,
    /// shown as a detrimental run modifier on the custom run screen.
    /// </summary>
    Bad
}



[HarmonyPatch(typeof(ModelDb))]
internal static class ModelDbPatches
{
    private static IEnumerable<CustomModifierModel>? _allCustomModifier;
    private static IEnumerable<CustomModifierModel> AllCustomModifier => 
        _allCustomModifier ??= ModelDb.AllAbstractModelSubtypes
                .Where(t => t.IsSubclassOf(typeof(ModifierModel)))
                .Select(t => (ModifierModel)ModelDb.Get(t))
                .OfType<CustomModifierModel>()
                .ToList();
    
    /// <summary>
    /// Appends custom good modifiers to <see cref="ModelDb.GoodModifiers"/>.
    /// </summary>
    [HarmonyPatch(nameof(ModelDb.GoodModifiers), MethodType.Getter)]
    [HarmonyPostfix]
    private static IReadOnlyList<ModifierModel> GoodModifiers(IReadOnlyList<ModifierModel> __result)
    {
        var before = AllCustomModifier
            .Where(e => e is { Alignment: ModifierAlignment.Good, SortOrder: < 0 })
            .OrderBy(e => e.SortOrder);
        var after = AllCustomModifier
            .Where(e => e is { Alignment: ModifierAlignment.Good, SortOrder: >= 0 })
            .OrderBy(e => e.SortOrder);
        return [.. before, .. __result, .. after];
    }
    /// <summary>
    /// Appends custom bad modifiers to <see cref="ModelDb.BadModifiers"/>.
    /// </summary>
    [HarmonyPatch(nameof(ModelDb.BadModifiers), MethodType.Getter)]
    [HarmonyPostfix]
    private static IReadOnlyList<ModifierModel> BadModifiers(IReadOnlyList<ModifierModel> __result)
    {
        var before = AllCustomModifier
            .Where(e => e is { Alignment: ModifierAlignment.Bad, SortOrder: < 0 })
            .OrderBy(e => e.SortOrder);
        var after = AllCustomModifier
            .Where(e => e is { Alignment: ModifierAlignment.Bad, SortOrder: >= 0 })
            .OrderBy(e => e.SortOrder);
        return [.. before, .. __result, .. after];
    }

    private static IReadOnlyList<IReadOnlySet<ModifierModel>>? _customMutuallyExclusive;
    
    /// <summary>
    /// Merged exclusivity groups derived from all custom modifiers' <see cref="CustomModifierModel.MutuallyExclusiveGroup"/>
    /// declarations. Uses union-find to merge transitively connected modifiers into single sets.
    /// Cached after first access.
    /// </summary>
    private static IReadOnlyList<IReadOnlySet<ModifierModel>> CustomMutuallyExclusive => 
        _customMutuallyExclusive ??= AllCustomModifier
                .Where(m => m.MutuallyExclusiveGroup.Any())
                .Aggregate(new List<HashSet<ModifierModel>>(), (groups, modifier) =>
                {
                    var members = modifier.MutuallyExclusiveGroup.Prepend(modifier).ToHashSet();
                    var overlapping = groups.Where(g => g.Overlaps(members)).ToList();
                    overlapping.ForEach(g => { groups.Remove(g); members.UnionWith(g); });
                    groups.Add(members);
                    return groups;
                })
                .Select(IReadOnlySet<ModifierModel> (g) => g)
                .ToList();

    /// <summary>
    /// Merges custom exclusivity groups into <see cref="ModelDb.MutuallyExclusiveModifiers"/>,
    /// combining with existing vanilla sets when a custom group overlaps with them.
    /// This ensures both the daily run roller and the custom run UI respect custom exclusivity.
    /// </summary>
    [HarmonyPatch(nameof(ModelDb.MutuallyExclusiveModifiers), MethodType.Getter)]
    [HarmonyPostfix]
    private static IReadOnlyList<IReadOnlySet<ModifierModel>> MutuallyExclusiveModifiers(
        IReadOnlyList<IReadOnlySet<ModifierModel>> __result)
    {
        if (CustomMutuallyExclusive.Count == 0) return __result;
        return CustomMutuallyExclusive
            .Aggregate(__result.Select(s => s.ToHashSet()).ToList(), (groups, customGroup) =>
            {
                var overlapping = groups.Where(g => g.Overlaps(customGroup)).ToList();
                var merged = customGroup.ToHashSet();
                overlapping.ForEach(g => { groups.Remove(g); merged.UnionWith(g); });
                groups.Add(merged);
                return groups;
            })
            .Select(IReadOnlySet<ModifierModel> (g) => g)
            .ToList();
    }
}