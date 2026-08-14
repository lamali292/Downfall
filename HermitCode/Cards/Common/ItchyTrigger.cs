using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Hermit.HermitCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Common;

public sealed class ItchyTrigger : HermitCardModel, IHasDeadOnEffect
{
    public ItchyTrigger() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(8, 2);
        WithVar("CostReduction", 1, 1);
    }

    protected override Artist Artist => Artist.Get<AlexMdle>();

    public Task DeadOnEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        var candidates = Owner.GetHand()
            .Where(c => c.EnergyCost.GetWithModifiers(CostModifiers.None) > 0)
            .ToList();

        if (candidates.Count <= 0) return Task.CompletedTask;
        var maxResolved = candidates.Max(c => c.EnergyCost.GetResolved());
        var topCost = candidates
                .Where(c => c.EnergyCost.GetResolved() == maxResolved)
                .ToList();

        var chosen = Owner.RunState.Rng.CombatCardSelection.NextItem(topCost);
        chosen?.EnergyCost.AddThisTurnOrUntilPlayed(-DynamicVars["CostReduction"].IntValue, true);
        return Task.CompletedTask;
    }


    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        await CommonActions.CardAttack(this, play).WithHermitGunHitFx().BeforeDamage(() =>
            {
                HermitSfx.PlayGun2();
                return Task.CompletedTask;
            })
            .Execute(ctx);
    }
}