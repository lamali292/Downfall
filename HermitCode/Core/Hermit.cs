using BaseLib.Utils;
using Downfall.DownfallCode.Abstract;
using Downfall.DownfallCode.Utils.Sound;
using Godot;
using Hermit.HermitCode.Cards.Basic;
using Hermit.HermitCode.Relics;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;

namespace Hermit.HermitCode.Core;

public class Hermit : DownfallCharacterModel
{
    private static readonly Color Color = new(0xCEA477FF);
    public override Color EnergyLabelOutlineColor => new(0xBA8900FF);
    public override string CharId => "Hermit";
    public override string ModId => HermitMainFile.ModId;
    public override Color NameColor => Color;
    public override Color LabOutlineColor => Color;
    public override Color DeckEntryCardColor => Color;
    public override float CardColorH => 0.1f;
    public override float CardColorS => 0.4f;
    public override float CardColorV => 1.2f;
    public override Color MapDrawingColor => new(0x7A4900FF);

    public override CharacterGender Gender => CharacterGender.Neutral;
    protected override CharacterModel? UnlocksAfterRunAs => null;
    public override int StartingHp => 70;
    public override int StartingGold => 99;

    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<StrikeHermit>(),
        ModelDb.Card<StrikeHermit>(),
        ModelDb.Card<StrikeHermit>(),
        ModelDb.Card<StrikeHermit>(),
        ModelDb.Card<DefendHermit>(),
        ModelDb.Card<DefendHermit>(),
        ModelDb.Card<DefendHermit>(),
        ModelDb.Card<DefendHermit>(),
        ModelDb.Card<Covet>(),
        ModelDb.Card<Snapshot>()
    ];
    

    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<OldLocket>()
    ];

    public override float AttackAnimDelay => 0.15f;

    public override float CastAnimDelay => 0.25f;

    public override CardPoolModel CardPool => ModelDb.CardPool<HermitCardPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<HermitPotionPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<HermitRelicPool>();
}

public class HermitRelicPool : DownfallRelicPool<Hermit>;

[Pool(typeof(HermitRelicPool))]
public abstract class HermitRelicModel(RelicRarity rarity, bool autoAdd = true)
    : DownfallRelicModel<Hermit>(rarity, autoAdd);

public abstract class HermitPowerModel(
    PowerType powerType = PowerType.Buff,
    PowerStackType powerStackType = PowerStackType.Counter) : DownfallPowerModel<Hermit>(powerType, powerStackType);

public class HermitPotionPool : DownfallPotionPool<Hermit>;

public class HermitCardPool : DownfallCardPool<Hermit>;

[Pool(typeof(HermitPotionPool))]
public abstract class HermitPotionModel(PotionRarity potionRarity, PotionUsage potionUsage, TargetType targetType) :
    DownfallPotionModel<Hermit>(potionRarity, potionUsage, targetType);