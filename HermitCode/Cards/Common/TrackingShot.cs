using BaseLib.Utils;
using Hermit.HermitCode.CustomEnums;
using Hermit.HermitCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Random;

namespace Hermit.HermitCode.Cards.Common;

public sealed class TrackingShot : HermitCardModel
{
    public TrackingShot() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(4, 2);
        this.WithRepeat(2);
        WithKeyword(HermitKeywords.Concentrate);
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        await CommonActions.CardAttack(this, play, 2).WithHermitGunHitFx().BeforeDamage(() =>
            {
                var i = Rng.Chaotic.NextInt(2);
                if (i == 0)
                    HermitSfx.PlayGun3();
                else
                    HermitSfx.PlayGun1();
                return Task.CompletedTask;
            })
            .Execute(ctx);
    }
}