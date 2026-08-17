using BaseLib.Utils;
using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves.Runs;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.CustomEnums;
using Snecko.SneckoCode.Interfaces;

namespace Snecko.SneckoCode.Relics;

[Pool(typeof(SneckoRelicPool))]
public class SneckoChoice : SneckoRelicModel, ISneckoPoolSupplier
{

    public SneckoChoice() : base(RelicRarity.Event)
    {
        WithTip(SneckoTip.Gift);
    }
    
    [SavedProperty] 
    // ReSharper disable once MemberCanBePrivate.Global
    public ModelId? CharacterId { get; private set; }
    
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