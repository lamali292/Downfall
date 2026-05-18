using BaseLib.Utils;
using Hermit.HermitCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Common;

public sealed class Roulette : HermitCardModel
{
    public Roulette() : base(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(16, 4);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        HermitSfx.PlaySpin();
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        HermitSfx.PlayGun2();
        await CommonActions.CardAttack(this, play).WithHermitGunHitFx()
            .Execute(ctx);
        if (Owner.PlayerCombatState == null) return;
        var hand = Owner.PlayerCombatState.Hand.Cards;
        var handSize = hand.Count;
        await CardCmd.DiscardAndDraw(ctx, hand, handSize);
    }
}