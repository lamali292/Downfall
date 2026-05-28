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
        this.WithRepeat(3);
        this.WithTip<Doubt>();
    }


    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        await CommonActions.CardAttack(this, play, DynamicVars.Repeat.IntValue)
            .WithHermitGunHitFx().BeforeDamage(() =>
            {
                HermitSfx.PlayGun3();
                return Task.CompletedTask;
            })
            .Execute(ctx);
        await DownfallCardCmd.GiveCard<Doubt>(Owner, PileType.Draw, CardPilePosition.Random);
    }
}