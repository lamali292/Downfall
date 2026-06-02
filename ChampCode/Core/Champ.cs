using Champ.ChampCode.Cards.Basic;
using Champ.ChampCode.Relics;
using Downfall.DownfallCode.Abstract;
using Downfall.DownfallCode.Utils.Sound;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;

namespace Champ.ChampCode.Core;

public class Champ : DownfallCharacterModel
{
    private static readonly Color Color = new(0x5E594FFF);
    public override Color EnergyLabelOutlineColor  => new(0x464203FF);
    public override string CharId => "Champ";
    public override string ModId => ChampMainFile.ModId;
    public override Color NameColor => Color;
    public override Color LabOutlineColor => Color;
    public override Color DeckEntryCardColor => Color;
    public override float CardColorH => 0.6f;
    public override float CardColorS => 0.5f;
    public override float CardColorV => 1.2f;
    public override Color MapDrawingColor => Color;

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
        ModelDb.Card<BerserkersShout>(),
        ModelDb.Card<DefensiveShout>(),
        ModelDb.Card<Execute>()
    ];


    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<ChampionsCrown>()
    ];

    public override float AttackAnimDelay => 0.15f;

    public override float CastAnimDelay => 0.25f;

    public override CardPoolModel CardPool => ModelDb.CardPool<ChampCardPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<ChampPotionPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<ChampRelicPool>();

    public override ModSoundEffect CharacterSelectSfxEntry => new(
        new ModSoundEntry("res://Champ/audio/character_select/STS_VO_TheChamp_3a.ogg", 1, 0.1f, 1, 10),
        new ModSoundEntry("res://Champ/audio/character_select/STS_VO_TheChamp_3b.ogg", 1, 0.1f, 1, 10)
    );

    /*
    public override CreatureAnimator GenerateAnimator(MegaSprite controller)
    {
        var idleNone = new AnimState("Idle", true);
        var idleBerserker = new AnimState("IdleBerserker", true);
        var idleDefensive = new AnimState("IdleDefensive", true);
        var idleUltimate = new AnimState("IdleUltimate", true);
        var idleGladiator = new AnimState("IdleGladiator", true);

        var hitNone = new AnimState("Hit");
        var hitBerserker = new AnimState("HitBerserker");
        var hitDefensive = new AnimState("HitDefensive");
        var hitUltimate = new AnimState("IdleUltimate");
        var hitGladiator = new AnimState("HitGladiator");

        hitNone.NextState = idleNone;
        hitBerserker.NextState = idleBerserker;
        hitDefensive.NextState = idleDefensive;
        hitUltimate.NextState = idleUltimate;
        hitGladiator.NextState = idleGladiator;

        var attackNone = new AnimState("Attack");
        var attackBerserker = new AnimState("Attack");
        var attackDefensive = new AnimState("Attack");
        var attackUltimate = new AnimState("Attack");
        var attackGladiator = new AnimState("Attack");

        attackNone.NextState = idleNone;
        attackBerserker.NextState = idleBerserker;
        attackDefensive.NextState = idleDefensive;
        attackUltimate.NextState = idleUltimate;
        attackGladiator.NextState = idleGladiator;

        var deadState = new AnimState("Idle");

        var animator = new CreatureAnimator(idleNone, controller);

        animator.AddAnyState("Dead", deadState);

        animator.AddAnyState("Attack", attackNone, IsInStance<ChampNoStance>);
        animator.AddAnyState("Attack", attackBerserker, IsInStance<ChampBerserkerStance>);
        animator.AddAnyState("Attack", attackDefensive, IsInStance<ChampDefensiveStance>);
        animator.AddAnyState("Attack", attackUltimate, IsInStance<ChampUltimateStance>);
        animator.AddAnyState("Attack", attackGladiator, IsInStance<ChampGladiatorStance>);

        animator.AddAnyState("Idle", idleNone, IsInStance<ChampNoStance>);
        animator.AddAnyState("Idle", idleBerserker, IsInStance<ChampBerserkerStance>);
        animator.AddAnyState("Idle", idleDefensive, IsInStance<ChampDefensiveStance>);
        animator.AddAnyState("Idle", idleUltimate, IsInStance<ChampUltimateStance>);
        animator.AddAnyState("Idle", idleGladiator, IsInStance<ChampGladiatorStance>);

        animator.AddAnyState("Hit", hitNone, IsInStance<ChampNoStance>);
        animator.AddAnyState("Hit", hitBerserker, IsInStance<ChampBerserkerStance>);
        animator.AddAnyState("Hit", hitDefensive, IsInStance<ChampDefensiveStance>);
        animator.AddAnyState("Hit", hitUltimate, IsInStance<ChampUltimateStance>);
        animator.AddAnyState("Hit", hitGladiator, IsInStance<ChampGladiatorStance>);


        return animator;

        bool IsInStance<T>() where T : ChampStanceModel
        {
            return ControllerToPlayer.TryGetValue(controller, out var player)
                   && ChampModel.IsInStance<T>(player);
        }
    }
    */
}

public class ChampRelicPool : DownfallRelicPool<Champ>;

public abstract class ChampRelicModel(RelicRarity rarity, bool autoAdd = true) : DownfallRelicModel<Champ>(rarity, autoAdd);

public abstract class ChampPowerModel(
    PowerType powerType = PowerType.Buff,
    PowerStackType powerStackType = PowerStackType.Counter) : DownfallPowerModel<Champ>(powerType, powerStackType);

public class ChampPotionPool : DownfallPotionPool<Champ>;

public class ChampCardPool : DownfallCardPool<Champ>;