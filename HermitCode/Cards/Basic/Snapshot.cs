using BaseLib.Abstracts;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Hermit.HermitCode.Cards.Ancient;
using Hermit.HermitCode.Core;
using Hermit.HermitCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Hermit.HermitCode.Cards.Basic;

public sealed class Snapshot : HermitCardModel, ITranscendenceCard
{
    public Snapshot() : base(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
    {
        WithDamage(5, 3);
        WithDeadOn();
    }

    protected override Artist Artist => Artist.Get<AlexMdle>();

    public override bool GainsBlock => true;

    public CardModel GetTranscendenceTransformedCard()
    {
        return ModelDb.Card<Crackshot>();
    }


    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay play)
    {
        // await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        var result = await CommonActions.CardAttack(this, play)
            .WithHermitGunHitFx().BeforeDamage(() =>
            {
                HermitSfx.PlayGun1();
                return Task.CompletedTask;
            })
            .Execute(ctx);
        var unblockedDamage = result.Results.SelectMany(e => e).Sum(e => e.TotalDamage + e.OverkillDamage);
        await HermitCmd.DeadOn(ctx, this, play, async () =>
        {
            await CreatureCmd.GainBlock(Owner.Creature, unblockedDamage, BlockProps.card, play);
        });
    }
}