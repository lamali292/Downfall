using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SlimeBoss.SlimeBossCode.Extensions;
using SlimeBoss.SlimeBossCode.Powers;

namespace SlimeBoss.SlimeBossCode.Slimes;

public class EvolutionSlime : SlimeModel
{
    private const int MaxLevel = 4;
    private const float ScalePerLevel = 0.15f;
    private int _level = 1;

    public override int? Level => _level;

    // From level 2 it hits all enemies.
    public override string CounterIconPath => _level >= 2 ? AoeAttackIconPath : base.CounterIconPath;

    public override float VisualScale => 0.7f + (_level - 1) * ScalePerLevel;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(4, DamageProps.nonCardUnpowered)
    ];

    protected override string SkinName => "torchhead";

    public override async Task Command(PlayerChoiceContext ctx, Creature? forcedTarget = null)
    {
        var attack = DamageCmd.Attack(DynamicVars.Damage.BaseValue).Unpowered().FromSlime(this);
        attack = _level < 2 ? forcedTarget == null ?
            attack.TargetingRandomOpponents(CombatState) : attack.Targeting(forcedTarget) : attack.TargetingAllOpponents(CombatState);

        var result = await attack.Execute(ctx);
        if (_level >= 3)
        {
            var dealt = result.Results.SelectMany(e => e).Sum(e => e.TotalDamage + e.OverkillDamage) / 2;
            await CreatureCmd.GainBlock(PetOwner, dealt, BlockProps.nonCardUnpowered, null);
        }
        if (_level >= 4)
        {
            await PowerCmd.Apply<PotencyPower>(ctx, Creature, 2, PetOwner, null);
        }
        if (_level < MaxLevel) _level++;
    }
}
