using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SlimeBoss.SlimeBossCode.Extensions;
using SlimeBoss.SlimeBossCode.Powers;

namespace SlimeBoss.SlimeBossCode.Slimes;

public class MuscleSlime : SlimeModel
{
    public override SlimeType SlimeType => SlimeType.Normal;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6, DamageProps.nonCardUnpowered)
    ];

    // NOTE: no scene/skin exists for this slime yet - reusing BruiserSlime's "attack" skin as a placeholder.
    public override void SetupSkins(MegaSprite spine, MegaSkeleton skeleton)
    {
        skeleton.SetSkin(skeleton.GetData().FindSkin("attack"));
        skeleton.SetSlotsToSetupPose();
    }

    // "Targets whichever enemy was attacked last during your turn" - looks up the most recent
    // CreatureAttackedEntry made by the owner this turn and re-targets its last hit's receiver.
    private Creature? GetLastAttackedEnemy()
    {
        return CombatManager.Instance.History.Entries
            .OfType<CreatureAttackedEntry>()
            .Where(e => e.Actor == PetOwner && e.HappenedThisTurn(CombatState))
            .SelectMany(e => e.DamageResults)
            .Select(e => e.Receiver)
            .LastOrDefault(e => e.IsAlive);
    }

    public override async Task Command(PlayerChoiceContext ctx, Creature? forcedTarget = null)
    {
        var attack = DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromSlime(this);
        var target = forcedTarget ?? GetLastAttackedEnemy();
        attack = target != null ? attack.Targeting(target) : attack.TargetingRandomOpponents(CombatState);
        await attack.Execute(ctx);
    }

    // "Gains 1 Potency this turn whenever you play an Attack" - reactive passive, not part of Command().
    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != PetOwner || cardPlay.Card.Type != CardType.Attack) return;
        await PowerCmd.Apply<MuscleSlimePotencyPower>(ctx, PetOwner, 1, PetOwner, null);
    }
}

public class MuscleSlimePotencyPower : CustomTemporaryPowerModelWrapper<MuscleSlime, PotencyPower>;
