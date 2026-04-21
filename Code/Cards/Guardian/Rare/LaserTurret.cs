using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Interfaces;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Guardian.Rare;

[Pool(typeof(GuardianCardPool))]
public class LaserTurret : GuardianCardModel, ITickCard, ICustomTickDuration
{
    public LaserTurret() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(5, 2);
        WithTip(DownfallTip.Stasis);
        WithTip(DownfallTip.Tick);
    }

    public int TickDuration => 4;


    public async Task OnTick(PlayerChoiceContext ctx)
    {
        var enemy = CombatState?.HittableEnemies.TakeRandom(1, CombatState.RunState.Rng.CombatTargets).FirstOrDefault();
        if (enemy == null) return;
        await CreatureCmd.Damage(ctx, enemy, DynamicVars.Damage, Owner.Creature);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await GuardianCmd.PutIntoStasis(this, ctx);
    }
}