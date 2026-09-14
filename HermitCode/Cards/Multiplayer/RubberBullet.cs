using BaseLib.Utils;
using Hermit.HermitCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Hermit.HermitCode.Cards.Multiplayer;

public class RubberBullet : HermitCardModel, IHasDeadOnEffect
{
    public RubberBullet() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(7, 2);
        WithVar("Increase", 7, 2);
    }

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    // The copy that replaced this card in a teammate's hand. Set by the first Dead On trigger of a
    // play; a second trigger (Snipe) must stack its damage increase on that copy, since this card
    // is no longer in combat by then, and must not hand the card off again.
    private CardModel? _handedOff;

    public async Task DeadOnEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var live = _handedOff ?? this;
        live.DynamicVars.Damage.UpgradeValueBy(live.DynamicVars["Increase"].IntValue);
        if (_handedOff != null) return;

        var player = Owner.RandomOtherTeammate;
        if (player == null) return;

        // TODO: use CreateCloneForPlayer on main / beta merge
        var clone = CreateClone();
        _handedOff = clone;
        clone._owner = player;
        clone.EnergyCost.AfterCardPlayedCleanup();
        clone.EnergyCost.EndOfTurnCleanup();
        await CardPileCmd.RemoveFromCombat(this);
        await CardPileCmd.Add(clone, PileType.Hand);
        HermitSfx.PlayReload();
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        // await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        await CommonActions.CardAttack(this, cardPlay).WithHermitGunHitFx().BeforeDamage(() =>
            {
                HermitSfx.PlayGun3();
                return Task.CompletedTask;
            })
            .Execute(ctx);
    }
}