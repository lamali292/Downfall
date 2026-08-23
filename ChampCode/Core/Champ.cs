using Champ.ChampCode.Cards.Basic;
using Champ.ChampCode.Relics;
using Downfall.DownfallCode.Abstract;
using Downfall.DownfallCode.Config;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;

namespace Champ.ChampCode.Core;

#pragma warning disable STS001
public class Champ : DownfallCharacterModel
#pragma warning restore STS001
{
    private static readonly Color Color = new(0x5E594FFF);
    public override Color EnergyLabelOutlineColor => new(0x464203FF);
    public override string CharId => "Champ";
    public override string ModId => ChampMainFile.ModId;
    public override Color NameColor => Color;
    public override Color LabOutlineColor => Color;
    public override Color DeckEntryCardColor => Color;
    public override float CardColorH => 0.6f;
    public override float CardColorS => 0.5f;
    public override float CardColorV => 1.2f;
    public override Color MapDrawingColor => Color;

    public override bool HideFromVanillaCharacterSelect => DownfallConfig.HideChamp;
    public override bool HideInCompendium => DownfallConfig.HideChamp;
    
    public override CharacterGender Gender => CharacterGender.Masculine;
    protected override CharacterModel? UnlocksAfterRunAs => null;
    public override int StartingHp => 80;
    public override int StartingGold => 99;

    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<StrikeChamp>(),
        ModelDb.Card<StrikeChamp>(),
        ModelDb.Card<StrikeChamp>(),
        ModelDb.Card<StrikeChamp>(),
        ModelDb.Card<DefendChamp>(),
        ModelDb.Card<DefendChamp>(),
        ModelDb.Card<DefendChamp>(),
        ModelDb.Card<DefendChamp>(),
        ModelDb.Card<BerserkersShout>(),
        ModelDb.Card<DefensiveShout>(),
        ModelDb.Card<Execute>()
    ];


    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<ChampionsCrown>()
    ];

    public override float AttackAnimDelay => 0.2f;

    public override float CastAnimDelay => 0.25f;

    public override CardPoolModel CardPool => ModelDb.CardPool<ChampCardPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<ChampPotionPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<ChampRelicPool>();
    
    public static string GetJumpAnimIfApplicable(CharacterModel character)
    {
        return character is not Champ ? "Attack" : "jumpAttack";
    }

    public static float GetJumpAttackDelayIfApplicable(CharacterModel character)
    {
        return character is not Champ ? character.AttackAnimDelay : 0.5f;
    }
}

public class ChampRelicPool : DownfallRelicPool<Champ>;

public abstract class ChampRelicModel(RelicRarity rarity, bool autoAdd = true)
    : DownfallRelicModel<Champ>(rarity, autoAdd);

public abstract class ChampPowerModel(
    PowerType powerType = PowerType.Buff,
    PowerStackType powerStackType = PowerStackType.Counter) : DownfallPowerModel<Champ>(powerType, powerStackType);

public class ChampPotionPool : DownfallPotionPool<Champ>;

public class ChampCardPool : DownfallCardPool<Champ>;

public abstract class ChampPotionModel(PotionRarity potionRarity, PotionUsage potionUsage, TargetType targetType) :
    DownfallPotionModel<Champ>(potionRarity, potionUsage, targetType);