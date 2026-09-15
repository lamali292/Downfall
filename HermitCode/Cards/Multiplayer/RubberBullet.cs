using BaseLib.Utils;
using Downfall.DownfallCode.Compatibility;
using Hermit.HermitCode.Core;
using Hermit.HermitCode.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Hermit.HermitCode.Cards.Multiplayer;

public class RubberBullet : HermitCardModel, IHasDeadOnEffect, IModifyCardPlayResultLocation
{
    public RubberBullet() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(7, 2);
        WithVar("Increase", 7, 2);
    }

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    // Runs before this card's own effect (see CardModel.OnPlayWrapper), same as every other
    // IModifyCardPlayResultLocation redirect (Combo, Feral, Reroute, ...) - so this composes with
    // them generically instead of RubberBullet needing to know about any of them by name: if
    // something else already committed this exact card to the owner's own hand (e.g. Combo), that
    // wins and we back off, whichever order the hooks happen to run in.
    public CardLocationCompatiblity ModifyCardPlayResultLocationCompability(CardModel card, bool isAutoPlay,
        ResourceInfo resources, CardLocationCompatiblity cardLocation)
    {
        if (card != this || !HermitCmd.HasActiveDeadOnEffect(card) || cardLocation is { PileType: PileType.Hand, Player: var p } && p == card.Owner) return cardLocation;
        var teammate = Owner.RandomOtherTeammate;
        return teammate == null ? cardLocation : new CardLocationCompatiblity(Player: teammate, PileType: PileType.Hand, Position: CardPilePosition.Bottom);
    }

    public Task AfterModifyingCardPlayResultLocationCompability(CardModel card, CardLocationCompatiblity cardLocation)
    {
        if (card == this && cardLocation.Player != card.Owner)
            HermitSfx.PlayReload();
        return Task.CompletedTask;
    }

    public Task DeadOnEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        DynamicVars.Damage.UpgradeValueBy(DynamicVars["Increase"].IntValue);
        return Task.CompletedTask;
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
