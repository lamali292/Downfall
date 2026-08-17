using BaseLib.Abstracts;
using BaseLib.Utils;
using Downfall.DownfallCode.Abstract;
using Downfall.DownfallCode.DynamicVars;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
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
    
        =>  [new FuncStringVar("borrowed", () => Character?.Title.GetFormattedText() ?? "")];

    [SavedProperty]
    // ReSharper disable once MemberCanBePrivate.Global
    public ModelId? CharacterId { get; private set; }
    
    
    public override LocString Title
    {
        get
        {
            var title = base.Title;
            if (Character != null)
                title.Add("character", Character.Title);
            return title;
        }
    }
    
    
    
    public void InitCharacter(CharacterModel character)
    {
        AssertMutable();   
        CharacterId = character.Id;
    }
    
    public override string PackedIconPath => Character?.IconTexturePath ?? base.PackedIconPath;
    protected override string PackedIconOutlinePath => Character?.IconOutlineTexturePath ?? base.PackedIconOutlinePath;
    protected override string BigIconPath => Character?.IconTexturePath ?? base.BigIconPath;
   

    private CharacterModel? Character => CharacterId == null ? null : ModelDb.GetByIdOrNull<CharacterModel>(CharacterId);
    
    public CardPoolModel? AddSneckoPool()
    {
        return Character?.CardPool;
    }
    
}