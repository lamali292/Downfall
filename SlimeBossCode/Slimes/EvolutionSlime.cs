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
    private const int MaxLevel = 5;
    private int _level = 1;
    // TODO show level and change model?
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(1, DamageProps.nonCardUnpowered),
        new DamageVar("Damage2", 4,  DamageProps.nonCardUnpowered)
    ];

    protected override string? SkinName => "poison";

    public override async Task Command(PlayerChoiceContext ctx, Creature? forcedTarget = null)
    {
        var damage = _level < 2 ? DynamicVars.Damage.BaseValue : DynamicVars["Damage2"].BaseValue;
        var attack =  DamageCmd.Attack(damage).FromSlime(this);
        attack = _level < 3 ? forcedTarget == null ? 
            attack.TargetingRandomOpponents(CombatState) : attack.Targeting(forcedTarget) : attack.TargetingAllOpponents(CombatState);
        
        var result = await attack.Execute(ctx);
        if (_level >= 4)
        {
            var dealt = result.Results.SelectMany(e => e).Sum(e => e.TotalDamage + e.OverkillDamage) / 2;
            await CreatureCmd.GainBlock(PetOwner, dealt, BlockProps.nonCardUnpowered, null);
        };
        if (_level >= 5)
        {
            await PowerCmd.Apply<PotencyPower>(ctx, Creature, 2, PetOwner, null);
        }
        if (_level < MaxLevel) _level++;
    }
}
