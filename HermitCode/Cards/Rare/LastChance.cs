using BaseLib.Utils;
using Hermit.HermitCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Rare;

public class LastChance : HermitCardModel
{
    public LastChance() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(7, 2);
        WithCards(3, 1);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        await CommonActions.CardAttack(this, cardPlay).BeforeDamage(() =>
        {
            HermitSfx.PlayGun3();
            return Task.CompletedTask;
        }).Execute(ctx);

        if (Owner.Creature.Powers.All(e => e.TypeForCurrentAmount != PowerType.Debuff)) return;
        await CommonActions.Draw(this, ctx);
    }
}