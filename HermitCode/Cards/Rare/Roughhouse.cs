using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Hermit.HermitCode.Core;
using Hermit.HermitCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace Hermit.HermitCode.Cards.Rare;

public class Roughhouse : HermitCardModel
{
    public Roughhouse() : base(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(22, 6);
        WithDeadOn();
    }

    protected override Artist Artist => Artist.Get<AlexMdle>();
    public override bool GainsBlock => true;

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay play)
    {
        // await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        var result = await CommonActions.CardAttack(this, play).WithHermitBluntHeavyHitFx()
            .Execute(ctx);
        var unblockedDamage = result.Results.SelectMany(e => e).Sum(e => e.TotalDamage + e.OverkillDamage);
        await HermitCmd.DeadOn(ctx, this, play, async () =>
        {
            await CreatureCmd.GainBlock(Owner.Creature, unblockedDamage, BlockProps.card, play);
        });
    }
}