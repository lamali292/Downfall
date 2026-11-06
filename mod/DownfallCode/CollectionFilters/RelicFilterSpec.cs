using Godot;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Screens.RelicCollection;
using MegaCrit.Sts2.Core.Saves;

namespace Downfall.DownfallCode.CollectionFilters;

/// <summary>
/// Filters for the relic collection: general toggles (shared / event / other), one toggle per unlocked ancient, one
/// per character (from <see cref="ModelDb.AllCharacters"/>, so modded characters need no entry here), and the
/// rarities that aren't already covered by those toggles (Ancient and Event are).
/// </summary>
public static class RelicFilterSpec
{
    private const string SharedIconPath = "res://images/atlases/ui_atlas.sprites/card/energy_colorless.tres";
    // The card library's misc-pool burst stands for "special" (events, tokens), the question mark for "anything else".
    private const string EventIconPath = "res://images/packed/card_library/pool_filter_other.png";
    private const string OtherIconPath = "res://images/ui/run_history/event.png";

    public static CollectionFilterSpec Build()
    {
        var characters = ModelDb.AllCharacters.ToList();
        var shared = ModelDb.RelicPool<SharedRelicPool>();
        var ancientRelics = GetUnlockedAncients().ToDictionary(a => a, OfferedRelicIds);

        // The game also lists Ancient/Event-rarity relics (Fresnel Lens, Looming Fruit, ...) in the shared pool,
        // so pool membership alone would show them as "shared" instead of under their ancient / event / other.
        bool IsShared(RelicModel relic) =>
            shared.AllRelicIds.Contains(relic.Id) && relic.Rarity is not (RelicRarity.Ancient or RelicRarity.Event);
        bool IsEvent(RelicModel relic) => relic.Rarity == RelicRarity.Event;
        bool IsCharacterRelic(RelicModel relic) => characters.Any(c => BelongsTo(relic, c));
        bool IsAncientRelic(RelicModel relic) => ancientRelics.Values.Any(ids => ids.Contains(relic.Id));
        bool IsOther(RelicModel relic) =>
            !IsShared(relic) && !IsEvent(relic) && !IsCharacterRelic(relic) && !IsAncientRelic(relic);

        var general = new List<PoolToggleSpec>
        {
            new("Shared", GD.Load<Texture2D>(SharedIconPath), new LocString("gameplay_ui", "DOWNFALL-RELIC_POOL_SHARED"),
                CollectionFilterSpec.Typed<RelicModel>(IsShared)),
            new("Event", GD.Load<Texture2D>(EventIconPath), new LocString("gameplay_ui", "DOWNFALL-RELIC_POOL_EVENT"),
                CollectionFilterSpec.Typed<RelicModel>(IsEvent)),
        };
        // Whatever none of the other toggles claims. Vanilla has nothing left over, so only show it when a mod does.
        if (ModelDb.AllRelics.Any(IsOther))
            general.Add(new PoolToggleSpec("Other", GD.Load<Texture2D>(OtherIconPath),
                new LocString("gameplay_ui", "DOWNFALL-RELIC_POOL_OTHER"), CollectionFilterSpec.Typed<RelicModel>(IsOther)));

        var ancients = ancientRelics.Select(pair => new PoolToggleSpec(pair.Key.Id.Entry, pair.Key.RunHistoryIcon,
            pair.Key.Title, CollectionFilterSpec.Typed<RelicModel>(relic => pair.Value.Contains(relic.Id)))).ToList();

        var characterToggles = characters.Select(character => new PoolToggleSpec(character.Id.Entry,
            character.IconTexture, character.Title,
            CollectionFilterSpec.Typed<RelicModel>(relic => BelongsTo(relic, character)))).ToList();

        return new CollectionFilterSpec
        {
            PoolRows = [general, ancients, characterToggles],
            Rarities =
            [
                Rarity(RelicRarity.Starter, null),
                Rarity(RelicRarity.Common, null),
                Rarity(RelicRarity.Uncommon, FilterColors.Uncommon),
                Rarity(RelicRarity.Rare, FilterColors.Rare),
                Rarity(RelicRarity.Shop, FilterColors.Other),
            ],
            IsDiscovered = model => SaveManager.Instance.Progress.DiscoveredRelics.Contains(model.Id),
            SearchText = model => model is RelicModel relic
                ? $"{relic.Title.GetFormattedText()} {relic.DynamicDescription.GetFormattedText().StripBbCode()}"
                : string.Empty,
            ApplyVisibility = (root, matches) => UpdateVisibility(root, matches),
        };
    }

    private static RaritySpec Rarity(RelicRarity rarity, Color? tint)
    {
        var key = rarity.ToString().ToUpperInvariant();
        // relic_collection's entries are the long category-header texts, which suit the tooltip but not the label.
        return new RaritySpec(new LocString("gameplay_ui", $"DOWNFALL-FILTER_RARITY_{key}").GetRawText(),
            new LocString("relic_collection", key), tint,
            CollectionFilterSpec.Typed<RelicModel>(relic => relic.Rarity == rarity));
    }

    /// <summary>Same set the collection's own Ancient category shows: only ancients the player has unlocked.</summary>
    private static List<AncientEventModel> GetUnlockedAncients()
    {
        var unlockState = SaveManager.Instance.GenerateUnlockStateFromProgress();
        return ModelDb.Acts.SelectMany(act => act.GetUnlockedAncients(unlockState))
            .Concat(unlockState.SharedAncients)
            .Distinct()
            .ToList();
    }

    private static HashSet<ModelId> OfferedRelicIds(AncientEventModel ancient) =>
        ancient.AllPossibleOptions.Select(option => option.Relic?.CanonicalInstance).OfType<RelicModel>()
            .Select(relic => relic.Id).ToHashSet();

    private static bool BelongsTo(RelicModel relic, CharacterModel character)
    {
        if (character.RelicPool.AllRelicIds.Contains(relic.Id))
            return true;

        if (relic.Rarity != RelicRarity.Starter)
            return false;

        // Unmapped starters fall back to Circlet in GetUpgradedStarterRelic, so only trust it for Starter-rarity relics.
        var touch = ModelDb.Relic<TouchOfOrobas>();
        return character.StartingRelics.Any(starter =>
            starter.Id == relic.Id || touch.GetUpgradedStarterRelic(starter).Id == relic.Id);
    }

    /// <summary>Returns whether anything underneath <paramref name="node"/> is still shown.</summary>
    private static bool UpdateVisibility(Node node, Func<AbstractModel, bool> matches)
    {
        if (node is NRelicCollectionEntry entry)
        {
            entry.Visible = matches(entry.relic);
            return entry.Visible;
        }

        var anyShown = false;
        foreach (var child in node.GetChildren())
            anyShown |= UpdateVisibility(child, matches);

        if (node is NRelicCollectionCategory category)
            category.Visible = anyShown;

        return anyShown;
    }
}
