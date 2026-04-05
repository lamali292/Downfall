using Downfall.Code.Abstract;
using Downfall.Code.Cards.Champ.Basic;
using Downfall.Code.Core.Champ;
using Downfall.Code.Relics.Champ;
using Godot;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Character;

public class Champ : DownfallCharacterModel
{
    private static readonly Color Color = new(0x5E594FFF);
    public override string CharId => "Champ";
    public override Color NameColor => Color;
    public override Color LabOutlineColor => Color;
    public override Color DeckEntryCardColor => Color;
    public override Color CardColor => Color;
    public override Color MapDrawingColor => Color;

    public override CharacterGender Gender => CharacterGender.Masculine;
    protected override CharacterModel? UnlocksAfterRunAs => null;
    public override int StartingHp => 72;
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

    public override CreatureAnimator GenerateAnimator(MegaSprite controller)
    {
        var idleNone      = new AnimState("Idle",          true);
        var idleBerserker = new AnimState("IdleBerserker", true);
        var idleDefensive = new AnimState("IdleDefensive", true);
        var idleUltimate  = new AnimState("IdleUltimate",  true);
        var idleGladiator = new AnimState("IdleGladiator", true);

        var hitNone      = new AnimState("Hit");
        var hitBerserker = new AnimState("HitBerserker");
        var hitDefensive = new AnimState("HitDefensive");
        var hitUltimate  = new AnimState("IdleUltimate");
        var hitGladiator = new AnimState("HitGladiator");
        
        hitNone.NextState      = idleNone;
        hitBerserker.NextState = idleBerserker;
        hitDefensive.NextState = idleDefensive;
        hitUltimate.NextState = idleUltimate;
        hitGladiator.NextState = idleGladiator;
        
        var attackNone      = new AnimState("Attack");
        var attackBerserker = new AnimState("Attack");
        var attackDefensive = new AnimState("Attack");
        var attackUltimate  = new AnimState("Attack");
        var attackGladiator = new AnimState("Attack");
        
        attackNone.NextState      = idleNone;
        attackBerserker.NextState = idleBerserker;
        attackDefensive.NextState = idleDefensive;
        attackUltimate.NextState = idleUltimate;
        attackGladiator.NextState  = idleGladiator;
        
        var deadState = new AnimState("Idle");

        var animator = new CreatureAnimator(idleNone, controller);

        animator.AddAnyState("Dead",   deadState);

        animator.AddAnyState("Attack", attackNone,      IsInStance<NoChampStance>);
        animator.AddAnyState("Attack", attackBerserker, IsInStance<BerserkerChampStance>);
        animator.AddAnyState("Attack", attackDefensive, IsInStance<DefensiveChampStance>);
        animator.AddAnyState("Attack", attackUltimate,  IsInStance<UltimateChampStance>);
        animator.AddAnyState("Attack", attackGladiator, IsInStance<GladiatorChampStance>);
    
        animator.AddAnyState("Idle", idleNone,      IsInStance<NoChampStance>);
        animator.AddAnyState("Idle", idleBerserker, IsInStance<BerserkerChampStance>);
        animator.AddAnyState("Idle", idleDefensive, IsInStance<DefensiveChampStance>);
        animator.AddAnyState("Idle", idleUltimate,  IsInStance<UltimateChampStance>);
        animator.AddAnyState("Idle", idleGladiator, IsInStance<GladiatorChampStance>);

        animator.AddAnyState("Hit", hitNone,      IsInStance<NoChampStance>);
        animator.AddAnyState("Hit", hitBerserker, IsInStance<BerserkerChampStance>);
        animator.AddAnyState("Hit", hitDefensive, IsInStance<DefensiveChampStance>);
        animator.AddAnyState("Hit", hitUltimate,  IsInStance<UltimateChampStance>);
        animator.AddAnyState("Hit", hitGladiator, IsInStance<GladiatorChampStance>);


        return animator;

        bool IsInStance<T>() where T : ChampStanceModel
        {
            return ControllerToPlayer.TryGetValue(controller, out var player)
                   && ChampModel.IsInStance<T>(player);
        }
    }
}