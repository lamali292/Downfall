using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Downfall.DownfallCode.DynamicVars;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Saves.Runs;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.CustomEnums;
using Snecko.SneckoCode.Interfaces;

namespace Snecko.SneckoCode.Relics;

[Pool(typeof(SneckoRelicPool))]
public class SneckoChoice : CustomRelicModel, ISneckoPoolSupplier
{
    public override RelicRarity Rarity => RelicRarity.Event;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.Static(SneckoTip.Gift)];

    protected override IEnumerable<DynamicVar> CanonicalVars

        => [new FuncStringVar("borrowed", () => Character?.Title.GetFormattedText() ?? "???")];

    [SavedProperty]
    // ReSharper disable once MemberCanBePrivate.Global
    public ModelId? CharacterId { get; private set; }

    public override LocString Title
    {
        get
        {
            var title = base.Title;
            title.Add("character", Character?.Title.GetFormattedText() ?? "???");
            return title;
        }
    }


    private string IconName => Id.Entry
        .RemovePrefix()
        .ToLowerInvariant();


    public override string PackedIconPath =>
        Character?.IconTexturePath ?? $"{IconName}.tres".TresRelicImagePath<Core.Snecko>();

    protected override string PackedIconOutlinePath => Character?.IconOutlineTexturePath ??
                                                       $"{IconName}_outline.tres".TresRelicImagePath<Core.Snecko>();

    protected override string BigIconPath =>
        Character?.IconTexturePath ?? "{IconName}.png".BigRelicImagePath<Core.Snecko>();


    private CharacterModel? Character =>
        CharacterId == null ? null : ModelDb.GetByIdOrNull<CharacterModel>(CharacterId);

    public CharacterModel? AddSneckoChar()
    {
        return Character;
    }


    public void InitCharacter(CharacterModel character)
    {
        AssertMutable();
        CharacterId = character.Id;
        RefreshNRelicNodes();
    }

    private void RefreshNRelicNodes()
    {
        NRun.Instance?.GlobalUi.RelicInventory.RelicNodes
            .FirstOrDefault(holder => holder.Relic.Model == this)
            ?.Relic
            .Reload();
    }
}