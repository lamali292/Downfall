using BaseLib.Utils;
using Hermit.HermitCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Multiplayer;

public class RubberBullet : HermitCardModel, IHasDeadOnEffect
{
    public RubberBullet() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(7, 2);
        WithVar("Increase", 7, 2);
    }
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        await CommonActions.CardAttack(this, cardPlay).WithHermitGunHitFx().BeforeDamage(() =>
            {
                HermitSfx.PlayGun3();
                return Task.CompletedTask;
            })
            .Execute(ctx);
    }

    public async Task DeadOnEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        DynamicVars.Damage.UpgradeValueBy(DynamicVars["Increase"].IntValue);
        var player = RunState?.Rng.CombatTargets.NextItem(RunState.Players.Where(e => e.Creature.IsAlive && e != Owner));
        if (player == null) return;

        var clone = CreateClone();
        clone._owner = player;
        await CardPileCmd.RemoveFromCombat(this);
        await CardPileCmd.Add(clone, PileType.Hand);
        HermitSfx.PlayReload();
    }
}