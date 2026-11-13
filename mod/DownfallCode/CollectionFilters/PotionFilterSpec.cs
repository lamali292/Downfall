using Godot;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.PotionPools;
using MegaCrit.Sts2.Core.Nodes.Screens.PotionLab;
using MegaCrit.Sts2.Core.Saves;

namespace Downfall.DownfallCode.CollectionFilters;

/// <summary>
/// Filters for the potion lab: shared / special / other, then one toggle per character (from
/// <see cref="ModelDb.AllCharacters"/>), and the Common / Uncommon / Rare rarities. Special potions (Event and Token
/// rarity) have their own toggle, so they get no rarity tickbox.
/// </summary>
public static class PotionFilterSpec
{
    private const string SharedIconPath = "res://images/atlases/ui_atlas.sprites/card/energy_colorless.tres";
    private const string SpecialIconPath = "res://images/packed/card_library/pool_filter_other.png";
    private const string OtherIconPath = "res://images/ui/run_history/event.png";

    public static CollectionFilterSpec Build()
    {
        var characters = ModelDb.AllCharacters.ToList();
        var shared = ModelDb.PotionPool<SharedPotionPool>();

        bool IsSpecial(PotionModel potion) => potion.Rarity is PotionRarity.Event or PotionRarity.Token;
        bool IsShared(PotionModel potion) => shared.AllPotionIds.Contains(potion.Id) && !IsSpecial(potion);
        bool IsCharacterPotion(PotionModel potion) => characters.Any(c => c.PotionPool.AllPotionIds.Contains(potion.Id));
        bool IsOther(PotionModel potion) => !IsShared(potion) && !IsSpecial(potion) && !IsCharacterPotion(potion);

        var general = new List<PoolToggleSpec>
        {
            new("Shared", GD.Load<Texture2D>(SharedIconPath), new LocString("gameplay_ui", "DOWNFALL-POTION_POOL_SHARED"),
                CollectionFilterSpec.Typed<PotionModel>(IsShared)),
            new("Special", GD.Load<Texture2D>(SpecialIconPath), new LocString("gameplay_ui", "DOWNFALL-POTION_POOL_SPECIAL"),
                CollectionFilterSpec.Typed<PotionModel>(IsSpecial)),
        };
        // Same as the relic filter: only worth a button when a mod puts potions outside every other group.
        if (ModelDb.AllPotions.Any(IsOther))
            general.Add(new PoolToggleSpec("Other", GD.Load<Texture2D>(OtherIconPath),
                new LocString("gameplay_ui", "DOWNFALL-POTION_POOL_OTHER"), CollectionFilterSpec.Typed<PotionModel>(IsOther)));

        var characterToggles = characters.Select(character => new PoolToggleSpec(character.Id.Entry,
            character.IconTexture, character.Title,
            CollectionFilterSpec.Typed<PotionModel>(potion => character.PotionPool.AllPotionIds.Contains(potion.Id)))).ToList();

        return new CollectionFilterSpec
        {
            PoolRows = [general, characterToggles],
            Rarities =
            [
                Rarity(PotionRarity.Common, null),
                Rarity(PotionRarity.Uncommon, FilterColors.Uncommon),
                Rarity(PotionRarity.Rare, FilterColors.Rare),
            ],
            IsDiscovered = model => SaveManager.Instance.Progress.DiscoveredPotions.Contains(model.Id),
            SearchText = model => model is PotionModel potion
                ? $"{potion.Title.GetFormattedText()} {potion.DynamicDescription.GetFormattedText().StripBbCode()}"
                : string.Empty,
            ApplyVisibility = (root, matches) => UpdateVisibility(root, matches),
        };
    }

    private static RaritySpec Rarity(PotionRarity rarity, Color? tint)
    {
        var key = rarity.ToString().ToUpperInvariant();
        return new RaritySpec(new LocString("gameplay_ui", $"DOWNFALL-FILTER_RARITY_{key}").GetRawText(),
            new LocString("potion_lab", key), tint,
            CollectionFilterSpec.Typed<PotionModel>(potion => potion.Rarity == rarity));
    }

    /// <summary>Returns whether anything underneath <paramref name="node"/> is still shown.</summary>
    private static bool UpdateVisibility(Node node, Func<AbstractModel, bool> matches)
    {
        if (node is NLabPotionHolder holder)
        {
            holder.Visible = matches(holder._model);
            return holder.Visible;
        }

        var anyShown = false;
        foreach (var child in node.GetChildren())
            anyShown |= UpdateVisibility(child, matches);

        if (node is NPotionLabCategory category)
            category.Visible = anyShown;

        return anyShown;
    }
}
