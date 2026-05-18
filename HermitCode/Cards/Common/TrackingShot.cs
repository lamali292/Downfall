using BaseLib.Utils;
using Hermit.HermitCode.CustomEnums;
using Hermit.HermitCode.Powers;
using HermitMod.Utility;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Common;

public sealed class TrackingShot : HermitCardModel
{
    public TrackingShot() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(4, 2);
        WithRepeat(2);
        WithKeyword(HermitKeywords.Concentrate);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await PowerCmd.Apply<ConcentrationPower>(ctx, Owner.Creature, 1, Owner.Creature, this);

        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);

        for (var i = 0; i < DynamicVars.Repeat.IntValue; i++)
        {
            if (i == 0)
                HermitSfx.PlayGun3();
            else
                HermitSfx.PlayGun1();
            await CommonActions.CardAttack(this, play).WithHermitGunHitFx()
                .Execute(ctx);
        }
    }
}