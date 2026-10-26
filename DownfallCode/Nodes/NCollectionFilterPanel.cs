using Downfall.DownfallCode.CollectionFilters;
using System.Reflection;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Screens.CardLibrary;

namespace Downfall.DownfallCode.Nodes;

/// <summary>
/// Sidebar for a compendium screen (relics, potions), styled like the card library's: a search box, then rows of
/// pool toggles, then a block of rarity tickboxes. What the toggles are and how entries are shown or hidden comes from
/// a <see cref="CollectionFilterSpec"/>, so this node knows nothing about the screen it filters.
/// Nothing selected in a group means "no filter" for that group; groups combine with AND.
/// </summary>
public partial class NCollectionFilterPanel : Control
{
    public const string NodeName = "DownfallCollectionFilters";

    private const string ScenePath = "res://Downfall/scenes/collection_filter/collection_filter_panel.tscn";
    private const string PoolToggleScenePath = "res://scenes/screens/card_library/library_pool_toggle.tscn";
    private const string RarityTickboxScenePath = "res://scenes/screens/card_library/rarity_tickbox.tscn";

    private static readonly FieldInfo ImageField = AccessTools.Field(typeof(NCardPoolFilter), "_image");
    private static readonly FieldInfo ReticleField = AccessTools.Field(typeof(NCardPoolFilter), "_controllerSelectionReticle");

    private const float ToggleSize = 64f;
    private const int MaxGridRows = 3;

    private readonly Dictionary<NCardPoolFilter, Func<AbstractModel, bool>> _poolFilters = new();
    private readonly Dictionary<NCardRarityTickbox, Func<AbstractModel, bool>> _rarityFilters = new();
    private readonly List<GridContainer> _poolGrids = new();

    private CollectionFilterSpec _spec = null!;
    private Control _screen = null!;
    private PackedScene _poolToggleScene = null!;
    private NSearchBar _searchBar = null!;
    private VBoxContainer _filters = null!;

    public static NCollectionFilterPanel Create(Control screen, CollectionFilterSpec spec)
    {
        var panel = GD.Load<PackedScene>(ScenePath).Instantiate<NCollectionFilterPanel>();
        panel._screen = screen;
        panel._spec = spec;
        panel.Name = NodeName;
        return panel;
    }

    public override void _Ready()
    {
        _poolToggleScene = GD.Load<PackedScene>(PoolToggleScenePath);
        _filters = GetNode<VBoxContainer>("%Filters");
        _searchBar = GetNode<NSearchBar>("%SearchBar");
        _searchBar.Connect(NSearchBar.SignalName.QueryChanged, Callable.From<string>(_ => Apply()));
        _searchBar.Connect(NSearchBar.SignalName.QuerySubmitted, Callable.From<string>(_ => Apply()));

        foreach (var row in _spec.PoolRows)
        {
            var grid = CreatePoolGrid();
            foreach (var toggle in row)
                AddPoolFilter(grid, toggle);
        }

        BuildRarityFilters();

        foreach (var grid in _poolGrids)
            FitToHeight(grid);
    }

    private GridContainer CreatePoolGrid()
    {
        var grid = new GridContainer { Columns = 4 };
        grid.AddThemeConstantOverride("h_separation", 0);
        grid.AddThemeConstantOverride("v_separation", 0);
        _filters.AddChild(grid);
        _poolGrids.Add(grid);
        return grid;
    }

    /// <summary>
    /// Keeps a grid within <see cref="MaxGridRows"/> rows of <see cref="ToggleSize"/>: past that it adds columns and
    /// shrinks every toggle to match (4 columns at full size, 5 at 80%, ...), the same fit BaseLib applies to the
    /// card library's character filters, so the sidebar doesn't grow with every character or ancient.
    /// </summary>
    private static void FitToHeight(GridContainer grid)
    {
        var toggles = grid.GetChildren().OfType<NCardPoolFilter>().ToList();
        var columns = 4;
        while (ToggleSize * (4f / columns) * MathF.Ceiling(toggles.Count / (float)columns) > ToggleSize * MaxGridRows)
            columns++;

        var scale = Vector2.One * (4f / columns);
        foreach (var filter in toggles)
        {
            filter.CustomMinimumSize *= scale;
            filter.Size *= scale;
            filter.PivotOffset *= scale;

            var image = (Control)ImageField.GetValue(filter)!;
            image.CustomMinimumSize *= scale;
            image.Size *= scale;
            image.PivotOffset *= scale;
            image.Position = (filter.Size - image.Size) * 0.5f;
            if (image.GetChildCount() > 0 && image.GetChild(0) is Control shadow)
            {
                shadow.CustomMinimumSize *= scale;
                shadow.Size *= scale;
                shadow.PivotOffset *= scale;
            }

            var reticle = (NSelectionReticle)ReticleField.GetValue(filter)!;
            reticle.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
            reticle.PivotOffset = reticle.Size / 2f;
            reticle.ZIndex = 10;
        }

        grid.Columns = columns;
    }

    private void AddPoolFilter(Node parent, PoolToggleSpec spec)
    {
        var filter = _poolToggleScene.Instantiate<NCardPoolFilter>();
        filter.Name = $"{spec.Name}Filter";
        parent.AddChild(filter);

        foreach (var rect in filter.FindChildren("*", nameof(TextureRect), true, false).OfType<TextureRect>())
            rect.Texture = spec.Icon;

        filter.Loc = spec.Tip;
        filter.IsSelected = false;
        filter.Connect(NCardPoolFilter.SignalName.Toggled, Callable.From<NCardPoolFilter>(_ => Apply()));
        _poolFilters[filter] = spec.Matches;
    }

    private void BuildRarityFilters()
    {
        var scene = GD.Load<PackedScene>(RarityTickboxScenePath);

        // Packed tight like the card library's rarity block, instead of spaced like the pool rows above it.
        var block = new VBoxContainer();
        block.AddThemeConstantOverride("separation", 0);
        _filters.AddChild(block);

        foreach (var rarity in _spec.Rarities)
        {
            var tickbox = scene.Instantiate<NCardRarityTickbox>();
            tickbox.CustomMinimumSize = new Vector2(200f, 38f);
            block.AddChild(tickbox);

            var label = tickbox.GetNode<Control>("Label");
            label.Modulate = rarity.Tint ?? Colors.White;
            label.AddThemeFontSizeOverride("font_size", 22);

            tickbox.Loc = rarity.Tip;
            tickbox.SetLabel(rarity.Label);
            tickbox.Connect(NTickbox.SignalName.Toggled, Callable.From<NTickbox>(_ => Apply()));
            _rarityFilters[tickbox] = rarity.Matches;
        }
    }

    /// <summary>Clears every selection. Called when the screen opens, since it rebuilds all entries.</summary>
    public void Reset()
    {
        foreach (var filter in _poolFilters.Keys)
            filter.IsSelected = false;
        foreach (var tickbox in _rarityFilters.Keys)
            tickbox.IsTicked = false;
        _searchBar.TextArea.Text = string.Empty;
    }

    private void Apply()
    {
        var pools = _poolFilters.Where(kv => kv.Key.IsSelected).Select(kv => kv.Value).ToList();
        var rarities = _rarityFilters.Where(kv => kv.Key.IsTicked).Select(kv => kv.Value).ToList();
        var query = NSearchBar.Normalize(_searchBar.Text.Trim());

        bool Matches(AbstractModel model) =>
            (pools.Count == 0 || pools.Any(pool => pool(model))) &&
            (rarities.Count == 0 || rarities.Any(rarity => rarity(model))) &&
            (query.Length == 0 || _spec.IsDiscovered(model) && MatchesSearch(model, query));

        _spec.ApplyVisibility(_screen, Matches);
    }

    private bool MatchesSearch(AbstractModel model, string query) =>
        NSearchBar.Normalize(NSearchBar.RemoveHtmlTags(_spec.SearchText(model))).Contains(query);
}
