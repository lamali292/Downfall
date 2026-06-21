using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Hermit.HermitCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Common;

public sealed class ItchyTrigger : HermitCardModel, IHasDeadOnEffect
{
    public ItchyTrigger() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(6, 2);
        WithVar("CostReduction", 1, 1);
    }

    protected override Artist Artist => Artist.Get<AlexMdle>();

    public Task DeadOnEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        Owner.GetHand()
            .OrderByDescending(e => e.EnergyCost.GetResolved())
            .Take(1)
            .FirstOrDefault()?
            .EnergyCost
            .AddThisTurn(-DynamicVars["CostReduction"].IntValue, true);
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