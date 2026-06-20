using BaseLib.Utils;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.CustomEnums;
using Guardian.GuardianCode.Interfaces;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Guardian.GuardianCode.Cards.Rare;

[Pool(typeof(GuardianCardPool))]
public class LaserTurret : GuardianCardModel, ITickCard, ICustomTickDuration
{
    public LaserTurret() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(5, 2);
        WithTip(GuardianTip.Stasis);
        WithTip(GuardianTip.Tick);
    }

    public int TickDuration => 4;


    public async Task OnTick(PlayerChoiceContext ctx)
    {
        var enemy = CombatState?.RunState.Rng.CombatTargets.NextItem(CombatState.HittableEnemies);
        if (enemy == null) return;
        await CreatureCmd.Damage(ctx, enemy, DynamicVars.Damage, Owner.Creature);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await GuardianCmd.PutIntoStasis(this, ctx, this);
    }
}