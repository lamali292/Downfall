using Downfall.DownfallCode.Abstract;
using Downfall.DownfallCode.Utils.Sound;
using Godot;
using Guardian.GuardianCode.Cards.Basic;
using Guardian.GuardianCode.Relics;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;

namespace Guardian.GuardianCode.Core;

public class Guardian : DownfallCharacterModel
{
    private static readonly Color Color = new(0xCA5B5BFF);
    public override Color EnergyLabelOutlineColor  => new(0x575044FF);
    public override string CharId => "Guardian";
    public override string ModId => GuardianMainFile.ModId;
    public override Color NameColor => Color;
    public override Color LabOutlineColor => Color;
    public override Color DeckEntryCardColor => Color;
    public override float CardColorH => 0.17f;
    public override float CardColorS => 1.5f;
    public override float CardColorV => 1.2f;
    public override Color MapDrawingColor => Color;

    public override CharacterGender Gender => CharacterGender.Masculine;
    protected override CharacterModel? UnlocksAfterRunAs => null;
    public override int StartingHp => 80;
    public override int StartingGold => 99;

    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<StrikeGuardian>(),
        ModelDb.Card<StrikeGuardian>(),
        ModelDb.Card<StrikeGuardian>(),
        ModelDb.Card<StrikeGuardian>(),
        ModelDb.Card<DefendGuardian>(),
        ModelDb.Card<DefendGuardian>(),
        ModelDb.Card<DefendGuardian>(),
        ModelDb.Card<DefendGuardian>(),
        ModelDb.Card<CurlUp>(),
        ModelDb.Card<TwinSlam>()
    ];

    public override ModSoundEffect CharacterSelectSfxEntry => new(
        new ModSoundEntry("res://Guardian/audio/character_select/STS_SFX_Guardian3Destroy_v2.ogg", 1, 0.1f, 1, 7)
    );


    protected override IEnumerable<string> ExtraAssetPaths =>
        GuardianModelDb.AllGems.Select(g => g.IconPath);

    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<BronzeGear>()
    ];

    public override float AttackAnimDelay => 0.15f;

    public override float CastAnimDelay => 0.25f;

    public override CardPoolModel CardPool => ModelDb.CardPool<GuardianCardPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<GuardianPotionPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<GuardianRelicPool>();


    /*
    public override CreatureAnimator GenerateAnimator(MegaSprite controller)
    {
        var idleNormal = new AnimState("idle", true);
        var idleDefensive = new AnimState("defensive", true);


        var animator = new CreatureAnimator(idleNormal, controller);
        animator.AddAnyState("Idle", idleNormal, IsInMode<GuardianNormalMode>);
        animator.AddAnyState("Idle", idleDefensive, IsInMode<GuardianDefensiveMode>);
        return animator;

        bool IsInMode<T>() where T : GuardianModeModel
        {
            return ControllerToPlayer.TryGetValue(controller, out var player)
                   && GuardianCmd.IsInMode<T>(player);
        }
    }
    */
}

public class GuardianRelicPool : DownfallRelicPool<Guardian>;

public abstract class GuardianRelicModel(RelicRarity rarity, bool autoAdd = true) : DownfallRelicModel<Guardian>(rarity, autoAdd);

public abstract class GuardianPowerModel(
    PowerType powerType = PowerType.Buff,
    PowerStackType powerStackType = PowerStackType.Counter) : DownfallPowerModel<Guardian>(powerType, powerStackType);

public class GuardianPotionPool : DownfallPotionPool<Guardian>;

public class GuardianCardPool : DownfallCardPool<Guardian>;

public abstract class GuardianPotionModel(PotionRarity potionRarity, PotionUsage potionUsage, TargetType targetType) :
    DownfallPotionModel<Guardian>(potionRarity, potionUsage, targetType);