using BaseLib.Utils;
using Hermit.HermitCode.History;
using Hermit.HermitCode.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Common;

public sealed class CalledShot : HermitCardModel
{
    private const int DrawAmount = 1;

    public CalledShot() : base(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(5, 2);
        WithKeyword(CardKeyword.Retain, UpgradeType.Add);
    }

    protected override bool ShouldGlowGoldInternal => LastPlayTriggeredDeadOn();

    private bool LastPlayTriggeredDeadOn()
    {
        var cardplay1 = CombatManager.Instance.History.Entries.OfType<DeadOnEntry>()
            .Where(e => e.HappenedThisTurn(CombatState) && e.Actor == Owner.Creature)
            .Select(e => e.CardPlay).LastOrDefault();
        var cardplay2 = CombatManager.Instance.History.CardPlaysFinished
            .Where(e => e.HappenedThisTurn(CombatState) && e.Actor == Owner.Creature)
            .Select(e => e.CardPlay).LastOrDefault();
        return cardplay2 != null && cardplay1 != null && cardplay2 == cardplay1;
    }


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        HermitSfx.PlayGun2();
        await CommonActions.CardAttack(this, play).WithHermitGunHitFx()
            .Execute(ctx);

        if (LastPlayTriggeredDeadOn()) await CardPileCmd.Draw(ctx, DrawAmount, Owner);
    }
}