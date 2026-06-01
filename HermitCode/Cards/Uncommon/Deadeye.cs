using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Hermit.HermitCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Hermit.HermitCode.Cards.Uncommon;

public sealed class Deadeye : HermitCardModel, IHasDeadOnEffect
{
    public Deadeye() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(5, 1);
        WithPower<StrengthPower>(2, 1);
    }

    protected override Artist Artist => Artist.Get<AlexMdle>();

    public async Task DeadOnEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CommonActions.ApplySelf<StrengthPower>(ctx, this);
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        await CommonActions.CardAttack(this, play)
            .WithHermitBluntHeavyHitFx()
            .Execute(ctx);
    }
}