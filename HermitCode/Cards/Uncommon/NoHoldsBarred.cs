using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Hermit.HermitCode.Powers;
using Hermit.HermitCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Uncommon;

public sealed class NoHoldsBarred : HermitCardModel
{
    public NoHoldsBarred() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        WithDamage(19, 4);
        WithPower<BruisePower>(5, 1);
        WithEnergy(1);
        this.WithPower<DrainedPower>(1, false);
    }

    protected override Artist Artist => Artist.Get<AlexMdle>();

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        await CommonActions.CardAttack(this, play)
            .WithHermitSlashHitFx()
            .Execute(ctx);
        await CommonActions.Apply<BruisePower>(ctx, this, play);
        await CommonActions.ApplySelf<DrainedPower>(ctx, this);
    }
}