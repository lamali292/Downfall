using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SlimeBoss.SlimeBossCode.Extensions;
using SlimeBoss.SlimeBossCode.Powers;

namespace SlimeBoss.SlimeBossCode.Slimes;

// "Evolves whenever it attacks, up to 4 times" - each Command() counts as an attack for leveling purposes,
// even though levels 1-4's actual payoff fires at end of turn (AfterSideTurnEnd), not on Command itself.
// Levels are cumulative (Lvl5 includes Lvl1-4's effects).
public class EvolutionSlime : SlimeModel
{
    private const int MaxLevel = 5;
    private int _level = 1;

    public override SlimeType SlimeType => SlimeType.Specialist;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(1, DamageProps.nonCardUnpowered),
        new("Damage2", 4)
    ];

    // NOTE: no scene/skin exists for this slime yet - reusing Insulting's "champ" skin as a placeholder.
    public override void SetupSkins(MegaSprite spine, MegaSkeleton skeleton)
    {
        skeleton.SetSkin(skeleton.GetData().FindSkin("champ"));
        skeleton.SetSlotsToSetupPose();
    }

    public override async Task Command(PlayerChoiceContext ctx, Creature? forcedTarget = null)
    {
        if (_level < MaxLevel) _level++;
        if (_level >= MaxLevel)
            await PowerCmd.Apply<PotencyPower>(ctx, PetOwner, 2, PetOwner, null);
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext ctx, CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (!participants.Contains(PetOwner)) return;

        var damage = _level >= 2 ? DynamicVars["Damage2"].BaseValue : DynamicVars.Damage.BaseValue;
        var attack = DamageCmd.Attack(damage).FromSlime(this);
        attack = _level >= 3 ? attack.TargetingAllOpponents(CombatState) : attack.TargetingRandomOpponents(CombatState);
        await attack.Execute(ctx);

        if (_level < 4) return;
        // Simplification: "half damage dealt" is approximated as half of the per-target damage value
        // (not the summed total across all enemies at level 3+) to avoid depending on attack result internals.
        await CreatureCmd.GainBlock(PetOwner, damage / 2, BlockProps.nonCardUnpowered, null);
    }
}
