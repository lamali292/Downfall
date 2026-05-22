using BaseLib.Utils;
using Hermit.HermitCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace Hermit.HermitCode.Cards.Basic;

public sealed class Snapshot : HermitCardModel, IHasDeadOnEffect
{
    private AttackCommand? _result;


    public Snapshot() : base(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
    {
        WithDamage(6, 2);
    }

    public async Task DeadOnEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        if (_result == null) return;
        var unblockedDamage = _result.Results.SelectMany(e => e).Sum(e => e.UnblockedDamage);
        await CreatureCmd.GainBlock(Owner.Creature, unblockedDamage, ValueProp.Move, play);
        _result = null;
    }


/*
    public CardModel GetTranscendenceTransformedCard()
    {
        return ModelDb.Card<OneFlash>();
    }
*/

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        HermitSfx.PlayGun1();

        _result = await CommonActions.CardAttack(this, play)
            .WithHermitGunHitFx()
            .Execute(ctx);
    }
}