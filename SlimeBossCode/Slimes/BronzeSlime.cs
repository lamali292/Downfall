using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SlimeBoss.SlimeBossCode.Extensions;

namespace SlimeBoss.SlimeBossCode.Slimes;

public class BronzeSlime : SlimeModel
{
    private int _skipTurns;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(10, DamageProps.monsterMove),
        new("Sleep", 2)
    ];

    public override SlimeType SlimeType => SlimeType.Specialist;

    public override CreatureAnimator GenerateAnimator(MegaSprite controller)
    {
        return SetupAnimationState(controller, "idle", hitName: "hit");
    }

    public override async Task Command(PlayerChoiceContext ctx)
    {
        if (_skipTurns > 0)
        {
            _skipTurns--;
            return;
        }

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromSlime(this)
            .TargetingAllOpponents(CombatState)
            .Execute(ctx);
        _skipTurns = DynamicVars["Sleep"].IntValue;
    }
}