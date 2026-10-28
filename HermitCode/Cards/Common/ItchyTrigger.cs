using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Hermit.HermitCode.Core;
using Hermit.HermitCode.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Common;

public sealed class ItchyTrigger : HermitCardModel
{
    public ItchyTrigger() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(8, 2);
        WithVar("CostReduction", 1, 1);
        WithDeadOn();
    }

    protected override Artist Artist => Artist.Get<AlexMdle>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay play)
    {
        // await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        await CommonActions.CardAttack(this, play).WithHermitGunHitFx().BeforeDamage(() =>
            {
                HermitSfx.PlayGun2();
                return Task.CompletedTask;
            })
            .Execute(ctx);
        await HermitCmd.DeadOn(ctx, this, play, () =>
        {
            var candidates = Owner.Hand
                .Where(c => c.EnergyCost.GetWithModifiers(CostModifiers.None) > 0)
                .ToList();

            if (candidates.Count <= 0) return Task.CompletedTask;
            var maxResolved = candidates.Max(c => c.EnergyCost.GetAmountToSpend());
            var topCost = candidates
                .Where(c => c.EnergyCost.GetAmountToSpend() == maxResolved)
                .ToList();

            var chosen = Owner.RunState.Rng.CombatCardSelection.NextItem(topCost);
            chosen?.EnergyCost.AddThisTurnOrUntilPlayed(-DynamicVars["CostReduction"].IntValue, true);
            return Task.CompletedTask;
        });
    }
}