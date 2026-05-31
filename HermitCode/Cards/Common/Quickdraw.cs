using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Hermit.HermitCode.Powers;
using Hermit.HermitCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Common;

public sealed class Quickdraw : HermitCardModel
{
    public Quickdraw() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(9, 2);
        WithCards(2, 1);
        this.WithPower<DrawFewerCardsNextTurnPower>(1, false);
    }

    protected override Artist Artist => Artist.Get<AlexMdle>();


    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        await CommonActions.CardAttack(this, play).WithHermitBluntLightHitFx()
            .Execute(ctx);
        await CommonActions.Draw(this, ctx);
        await CommonActions.ApplySelf<DrawFewerCardsNextTurnPower>(ctx, this);
    }
}