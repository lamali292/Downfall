using Godot;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.CollectionFilters;

/// <summary>One icon toggle in a pool row. Selecting several in a panel shows items matching any of them.</summary>
public sealed record PoolToggleSpec(string Name, Texture2D Icon, LocString Tip, Func<AbstractModel, bool> Matches);

/// <summary>One rarity tickbox. <paramref name="Tint"/> colors its label; null keeps the default.</summary>
public sealed record RaritySpec(string Label, LocString Tip, Color? Tint, Func<AbstractModel, bool> Matches);

/// <summary>
/// Everything <see cref="Nodes.NCollectionFilterPanel"/> needs to filter one compendium screen, so the panel itself
/// knows nothing about relics or potions. Built per screen by <see cref="RelicFilterSpec"/> / <see cref="PotionFilterSpec"/>.
/// </summary>
public sealed class CollectionFilterSpec
{
    /// <summary>Rows of pool toggles, top to bottom. Each row is its own grid, so rows keep their grouping.</summary>
    public required IReadOnlyList<IReadOnlyList<PoolToggleSpec>> PoolRows { get; init; }

    public required IReadOnlyList<RaritySpec> Rarities { get; init; }

    /// <summary>Undiscovered entries never match a search, so searching can't reveal them.</summary>
    public required Func<AbstractModel, bool> IsDiscovered { get; init; }

    /// <summary>Title plus description, used for the search box.</summary>
    public required Func<AbstractModel, string> SearchText { get; init; }

    /// <summary>
    /// Shows or hides the screen's entries and now-empty categories for a predicate.
    /// Only toggles <see cref="CanvasItem.Visible"/>, so the screen's own loading and focus logic stay untouched.
    /// </summary>
    public required Action<Node, Func<AbstractModel, bool>> ApplyVisibility { get; init; }

    /// <summary>Adapts a typed predicate to the untyped one the panel works with.</summary>
    public static Func<AbstractModel, bool> Typed<T>(Func<T, bool> predicate) where T : AbstractModel =>
        model => model is T typed && predicate(typed);
}
