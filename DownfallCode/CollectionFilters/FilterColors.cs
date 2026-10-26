using Godot;

namespace Downfall.DownfallCode.CollectionFilters;

/// <summary>
/// The tints the card library gives its rarity labels, reused for relics and potions, which have no colors of their
/// own in the base game. Common stays untinted.
/// </summary>
public static class FilterColors
{
    public static readonly Color Uncommon = new(0.529412f, 0.807843f, 0.921569f);
    public static readonly Color Rare = new(0.937255f, 0.784314f, 0.317647f);

    /// <summary>The card library's "other" purple; used for relic Shop.</summary>
    public static readonly Color Other = new(0.933333f, 0.509804f, 0.933333f);
}
