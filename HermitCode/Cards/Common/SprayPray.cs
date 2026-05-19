using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using Hermit.HermitCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Hermit.HermitCode.Cards.Common;

public sealed class SprayPray : HermitCardModel
{
    public SprayPray() : base(1, CardType.Attack, CardRarity.Common, TargetType.RandomEnemy)
    {
        WithDamage(4, 1);
        WithRepeat(3);
        WithTip(typeof(Doubt));
    }


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        HermitSfx.PlayGun3();
        await CommonActions.CardAttack(this, play, DynamicVars.Repeat.IntValue)
            .WithHermitGunHitFx()
            .Execute(ctx);
        await DownfallCardCmd.GiveCard<Doubt>(Owner, PileType.Draw, CardPilePosition.Random);
    }
}